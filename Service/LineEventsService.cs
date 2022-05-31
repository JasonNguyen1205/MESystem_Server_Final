using MESystem.Data.LineControl;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MESystem.Data
{
    public class LineEventsService
    {
        private readonly LisDbContext _context;
        public LineEventsService(LisDbContext context)
        {
            _context = context;
        }

        public Task<vLineEvent[]> GetEventsByTime(int filter)
        {
            if (filter == 0)
            {
                return _context.LineEvents
                               .OrderBy(o => o.ProductionLine).ThenByDescending(n => n.StartTime)
                               .AsNoTracking()
                               .ToArrayAsync();
            }
            else if (filter == 1)
            {
                return _context.LineEvents
                               .Where(le => le.ProductionLine == "Assembly 6")
                               .OrderBy(n => n.StartTime)
                               .AsNoTracking()
                               .ToArrayAsync();
            }
            else 
            {
                return _context.LineEvents
                               .Where(s => s.StartTime >= DateTime.Today)
                               .OrderBy(o => o.ProductionLine).ThenByDescending(n => n.StartTime)
                               .AsNoTracking()
                               .ToArrayAsync();
            }
        }
        
        public Task<LastLineState[]> GetLastEvents()
        {
            var result = _context.LastLineStates
                                 .OrderBy(o => o.ProductionLine)
                                 .AsNoTracking()
                                 .ToArrayAsync();

            return result;
        }

        public async void AddComment(vLineEvent lineEvent)
        {
            int newCommentId = 0;
            var newComment = new LineComments()
            {
                LineID = lineEvent.LineId,
                Comment = lineEvent.Comment,
                CommentEN = lineEvent.CommentEN
            };

            _context.LineComment
                .Add(newComment);
            await _context.SaveChangesAsync();
            newCommentId = await GetCommentAsync(lineEvent.LineId);
            UpdateCommentID(lineEvent.Id, newCommentId);
        }

        public async Task<int> GetCommentAsync(int lineID)
        {
            int commentID = 0;
            var result = await _context.LineComment
                 .Where(s => s.LineID == lineID)
                 .OrderByDescending(o => o.Id)
                 .Take(1)
                 .AsNoTracking()
                 .ToListAsync();
            foreach (var row in result)
            {
                commentID = row.Id;
            }
            return commentID;
        }

        public async void UpdateComment(vLineEvent lineEvent)
        {
            var updateQuery = await _context.LineComment
                .FirstAsync(c => c.Id == lineEvent.CommentId && c.LineID == lineEvent.LineId);
            updateQuery.Comment = lineEvent.Comment;
            updateQuery.CommentEN = lineEvent.CommentEN;

            if (updateQuery != null && lineEvent.Comment != "")
            {
                _context.LineComment
                    .Update(updateQuery);
                await _context.SaveChangesAsync();
            }
            else 
            {
                _context.LineComment
                    .Remove(updateQuery);
                await _context.SaveChangesAsync();
            }
        }

        private async void UpdateCommentID(int lineId, int newCommentId)
        {
            var updateQuery = await _context.LineEventComment
                .FirstAsync(c => c.Id == lineId);
            updateQuery.CommentId = newCommentId;

            if (updateQuery != null)
            {
                _context.LineEventComment
                    .Update(updateQuery);
                await _context.SaveChangesAsync();
            }
        }

    }
}
