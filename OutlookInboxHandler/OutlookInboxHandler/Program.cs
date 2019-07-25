using Microsoft.Office.Interop.Outlook;
using System;
using System.Runtime.InteropServices;

namespace OutlookInboxHandler
{
    class Program
    {
        
        static void Main(string[] args)
        {
            NameSpace NS = (Marshal.GetActiveObject("Outlook.Application") as Application).GetNamespace("MAPI");

            Folder folder = (Folder)NS.Folders["frbgd7@mail.ru"].Folders["test"];

            Items box = folder.Items;

            ItemHandler itemHandler = new ItemHandler();
            itemHandler.Start(box);

            

        }

        public class ItemHandler
        {
            private void Box_ItemAdd(object Item)
            {
                Console.WriteLine("Incoming Message");
            }

            public void Start(Items box)
            {

                (new ComAwareEventInfo(typeof(ItemsEvents_Event), "ItemAdd")).AddEventHandler(box, new ItemsEvents_ItemAddEventHandler(this.Box_ItemAdd));
            }
        }
    }
}
