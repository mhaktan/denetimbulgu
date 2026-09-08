using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.ActionProgressNotes.Dto;

namespace DenetimBulgu.ActionProgressNotes
{
    public interface IActionProgressNoteAppService : IAsyncCrudAppService<
        ActionProgressNoteDto,
        long,
        PagedActionProgressNoteResultRequestDto,
        CreateActionProgressNoteDto,
        ActionProgressNoteDto>
    {
        List<GroupCountDto> GetGroupedCount(ActionProgressNoteGroupedCountInput input);
    }
}
