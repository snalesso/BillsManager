using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.Providers
{
    public interface ITagsProvider
    {
        uint GetLastTagID();

        IEnumerable<Tag> GetAll();

        bool Add(Tag tag);

        bool Edit(Tag tag);
        //bool Edit(IEnumerable<Tag> tags); // TODO: obsolete?

        bool Delete(Tag tag);
        //bool Delete(IEnumerable<Tag> tags); // TODO: obsolete?
    }
}         