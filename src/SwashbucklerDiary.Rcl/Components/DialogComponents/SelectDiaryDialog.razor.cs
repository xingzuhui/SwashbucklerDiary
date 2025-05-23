using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectDiaryDialog : DialogComponentBase
    {
        private readonly string scrollContainerId = $"scroll-container-{Guid.NewGuid():N}";

        private bool showSearch;

        private string? search;

        private List<DiaryModel> items = [];

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        public List<TagModel> Tags { get; set; } = [];

        [Parameter]
        public EventCallback<List<TagModel>> TagsChanged { get; set; }

        [Parameter]
        public EventCallback<DiaryModel> OnOK { get; set; }

        [Parameter]
        public EventCallback OnAfterShowContent { get; set; }

        private async void HandleOnBeforeShowContent()
        {
            search = string.Empty;
            showSearch = false;
            await UpdateItemsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateItemsAsync()
        {
            Expression<Func<DiaryModel, bool>> exp = GetExpression();
            items = await DiaryService.QueryDiariesAsync(exp);
        }

        private Expression<Func<DiaryModel, bool>> GetExpression()
        {
            Expression<Func<DiaryModel, bool>>? exp = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                Expression<Func<DiaryModel, bool>> expSearch
                    = it => (it.Title ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase)
                    || (it.Content ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase);
                exp = exp.And(expSearch);
            }

            if (exp == null)
            {
                return it => true;
            }

            return exp;
        }
    }
}