using System;
namespace WebAPI.Models
{
    /// <summary>
    /// IStateFactory<Issue, IssueState>便与测试.
    /// create方法初始化一个IssueState实例,复制issue中的数据
    /// </summary>
    public class IssueStateFactory : IStateFactory<Issue, IssueState>
    {
        private readonly IssueLinkFactory _links;
 
        public IssueStateFactory(IssueLinkFactory links)
        {
            _links = links;
        }
 
        public IssueState Create(Issue issue)
        {
            var model = new IssueState
                {
                    Id = issue.Id,
                    Title = issue.Title,
                    Description = issue.Description,
                    Status = Enum.GetName(typeof(IssueStatus), issue.Status)
                };
 
            //add hypermedia
            //添加self和Transition
            model.Links.Add(_links.Self(issue.Id));
            model.Links.Add(_links.Transition(issue.Id));
            
            switch (issue.Status) {
                case IssueStatus.Closed:
                    model.Links.Add(_links.Open(issue.Id));
                    break;
                case IssueStatus.Open:
                    model.Links.Add(_links.Close(issue.Id));
                    break;
            }
                
            return model;
        }
    }
}