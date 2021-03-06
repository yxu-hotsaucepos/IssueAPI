﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class IssueController : ApiController
    {
        private  IIssueStore _store;
        private IssueStateFactory _stateFactory;
        private  IssueLinkFactory _linkFactory;

        public IssueController()
        {
        
        }

        public IssueController(IIssueStore store, IssueStateFactory stateFactory, IssueLinkFactory linkFactory)
        {
            _store = store;
            _stateFactory = stateFactory;
            _linkFactory = linkFactory;
        }

        public async Task<HttpResponseMessage> Get()
        {
            _store = new InMemoryIssueStore();
            IssueLinkFactory links = new IssueLinkFactory(this.Request, this.GetType());
            _stateFactory = new IssueStateFactory(links);

            var issues = await _store.FindAsync();
            var issuesState = new IssuesState();
            issuesState.Issues = issues.Select(i => _stateFactory.Create(i));
            issuesState.Links.Add(new Link { Href = Request.RequestUri, Rel = LinkFactory.Rels.Self });
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        }

        public async Task<HttpResponseMessage> Get(string id)
        {
            _store = new InMemoryIssueStore();
            IssueLinkFactory links = new IssueLinkFactory(this.Request, this.GetType());
            _stateFactory = new IssueStateFactory(links);

            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(issue));
        }

        public async Task<HttpResponseMessage> GetSearch(string searchText)
        {
            var issues = await _store.FindAsyncQuery(searchText);
            var issuesState = new IssuesState();
            issuesState.Issues = issues.Select(i => _stateFactory.Create(i));
            issuesState.Links.Add(new Link { Href = Request.RequestUri, Rel = LinkFactory.Rels.Self });
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        }

        public async Task<HttpResponseMessage> Post(dynamic newIssue)
        {
            var issue = new Issue { Title = newIssue.title, Description = newIssue.description };
            await _store.CreateAsync(issue);
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = _linkFactory.Self(issue.Id).Href;
            return response;
        }

        public async Task<HttpResponseMessage> Patch(string id, dynamic issueUpdate)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            foreach (JProperty prop in issueUpdate)
            {
                if (prop.Name == "title")
                    issue.Title = prop.Value.ToObject<string>();
                else if (prop.Name == "description")
                    issue.Description = prop.Value.ToObject<string>();
            }
            await _store.UpdateAsync(issue);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public async Task<HttpResponseMessage> Delete(string id)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            await _store.DeleteAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>();
            fakeIssues.Add(new Issue { Id = "1", Title = "An issue", Description = "This is an issue", Status = IssueStatus.Open });
            fakeIssues.Add(new Issue { Id = "2", Title = "Another issue", Description = "This is another issue", Status = IssueStatus.Closed });
            return fakeIssues;
        }
    }
}
