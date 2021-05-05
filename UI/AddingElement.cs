using System.Collections.Generic;

namespace GuideCreator.UI
{
    internal interface IInteractiveElement
    {
        Dictionary<string, dynamic> GetContent();
    }
}
