using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebAPI.Models
{
    /// <summary>
    /// 定义了问题创建,获取和保存.异步操作
    /// </summary>
    public interface IIssueStore
    {
        Task<IEnumerable<Issue>> FindAsync();
        Task<Issue> FindAsync(string issueId);
        Task<IEnumerable<Issue>> FindAsyncQuery(string searchText);
        Task UpdateAsync(Issue issue);
        Task DeleteAsync(string issueId);
        Task CreateAsync(Issue issue);
    }
}
