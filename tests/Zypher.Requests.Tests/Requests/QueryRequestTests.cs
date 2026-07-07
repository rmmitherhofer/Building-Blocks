using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Requests;

namespace Zypher.Requests.Tests.Requests;

public class QueryRequestTests
{
    [Fact(DisplayName =
        "Given a new query request, " +
        "When it is created, " +
        "Then defaults are set")]
    [Trait("Type", nameof(QueryRequest))]
    public async Task QueryRequest_Defaults_AreSet()
    {
        //Given
        var request = new QueryRequest();

        //When
        var filter = request.Filter;

        //Then
        filter.Should().BeNull();
        request.Sort.Should().NotBeNull().And.BeEmpty();
        request.Pagination.Should().NotBeNull();
        request.Includes.Should().BeNull();
        request.Metadata.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a query request, " +
        "When properties are set, " +
        "Then they are persisted")]
    [Trait("Type", nameof(QueryRequest))]
    public async Task QueryRequest_Setters_PersistValues()
    {
        //Given
        var request = new QueryRequest();
        var filter = new FilterGroup();
        var sort = new List<SortRequest> { new() { Field = "name" } };
        var pagination = new PaginationRequest { Page = 2 };
        var includes = new List<string> { "roles" };
        var metadata = new Dictionary<string, object> { ["tenant"] = "acme" };

        //When
        request.Filter = filter;
        request.Sort = sort;
        request.Pagination = pagination;
        request.Includes = includes;
        request.Metadata = metadata;

        //Then
        request.Filter.Should().BeSameAs(filter);
        request.Sort.Should().BeSameAs(sort);
        request.Pagination.Should().BeSameAs(pagination);
        request.Includes.Should().BeSameAs(includes);
        request.Metadata.Should().BeSameAs(metadata);
        await Task.CompletedTask;
    }
}
