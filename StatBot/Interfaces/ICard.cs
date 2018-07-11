using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace StatBot.Interfaces
{
    interface ICard
    {
        List<string> ButtonsName { get; set; }
        CardAction Action { get; set; }
        Activity OnClick(Activity activity);
    }
}
