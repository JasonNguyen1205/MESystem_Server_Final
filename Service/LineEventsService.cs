using System.Data;

using MESystem.Data.LineControl;

using Microsoft.EntityFrameworkCore;

namespace MESystem.Service;

public class LineEventsService
{
    private readonly LisDbContext _context;
    public LineEventsService(LisDbContext context)
    {
        _context=context;
    }

    public Task<vLineEvent[]> GetEventsByTime(int filter)
    {
        if(filter==0)
        {
            return _context.LineEvents
                           .OrderBy(o => o.ProductionLine).ThenByDescending(n => n.StartTime)
                           .AsNoTracking()
                           .ToArrayAsync();
        }
        else if(filter==1)
        {
            return _context.LineEvents
                           .Where(le => le.ProductionLine=="Assembly 6")
                           .OrderBy(n => n.StartTime)
                           .AsNoTracking()
                           .ToArrayAsync();
        }
        else
        {
            return _context.LineEvents
                           .Where(s => s.StartTime>=DateTime.Today)
                           .OrderBy(o => o.ProductionLine).ThenByDescending(n => n.StartTime)
                           .AsNoTracking()
                           .ToArrayAsync();
        }
    }

    public Task<LastLineState[]> GetLastEvents()
    {
        Task<LastLineState[]>? result = _context.LastLineStates
                             .OrderBy(o => o.ProductionLine)
                             .AsNoTracking()
                             .ToArrayAsync();

        return result;
    }

    public async void AddComment(vLineEvent lineEvent)
    {
        LineComments? newComment = new()
        {
            LineID=lineEvent.LineId,
            Comment=lineEvent.Comment,
            CommentEN=lineEvent.CommentEN
        };

        _=_context.LineComment
            .Add(newComment);
        _=await _context.SaveChangesAsync();
        var newCommentId = await GetCommentAsync(lineEvent.LineId);
        UpdateCommentID(lineEvent.Id, newCommentId);
    }

    public async Task<int> GetCommentAsync(int lineID)
    {
        var commentID = 0;
        List<LineComments>? result = await _context.LineComment
             .Where(s => s.LineID==lineID)
             .OrderByDescending(o => o.Id)
             .Take(1)
             .AsNoTracking()
             .ToListAsync();
        foreach(LineComments? row in result)
        {
            commentID=row.Id;
        }
        return commentID;
    }

    public async void UpdateComment(vLineEvent lineEvent)
    {
        LineComments? updateQuery = await _context.LineComment
            .FirstAsync(c => c.Id==lineEvent.CommentId&&c.LineID==lineEvent.LineId);
        updateQuery.Comment=lineEvent.Comment;
        updateQuery.CommentEN=lineEvent.CommentEN;

        if(updateQuery!=null&&lineEvent.Comment!="")
        {
            _=_context.LineComment
                .Update(updateQuery);
            _=await _context.SaveChangesAsync();
        }
        else
        {
            _=_context.LineComment
                .Remove(updateQuery);
            _=await _context.SaveChangesAsync();
        }
    }

    private async void UpdateCommentID(int lineId, int newCommentId)
    {
        LineEvent? updateQuery = await _context.LineEventComment
            .FirstAsync(c => c.Id==lineId);
        updateQuery.CommentId=newCommentId;

        if(updateQuery!=null)
        {
            _=_context.LineEventComment
                .Update(updateQuery);
            _=await _context.SaveChangesAsync();
        }
    }

}
