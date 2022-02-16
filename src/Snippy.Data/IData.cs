using Snippy.Models;
using System.Collections.Generic;

namespace Snippy.Data;

public interface IData
{
    ShortURL RegisterClick(ClickRequest request);

    IList<ShortURL> GetURLs(string IdentId);
    IList<string> GetKeys(string Prefix, int limit = 0);
    Owner GetOwner(string IdentId);
    bool RegisterUrl(ShortURL Url, Owner owner);
    bool IsIdAvail(string UrlKey);
    bool DeleteShort(string UrlKey);
}
