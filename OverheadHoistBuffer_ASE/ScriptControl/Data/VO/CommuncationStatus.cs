using com.mirle.ibg3k0.bcf.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class CommuncationInfo : PropertyChangedVO
    {
        public string Name;
        public string Getway_IP;
        public string Remote_IP;
        private bool iscommunactionsuccess;
        public bool IsCommunactionSuccess
        {
            get { return iscommunactionsuccess; }
            set
            {
                iscommunactionsuccess = value;
                if (iscommunactionsuccess != value)
                {
                   // OnPropertyChanged(value, BCFUtility.getPropertyName(() => this.IsCommunactionSuccess));
                }
            }
        }
        private bool Isconnectinosuccess;
        public bool IsConnectinoSuccess
        {
            get { return Isconnectinosuccess; }
            set
            {
                Isconnectinosuccess = value;
                if (Isconnectinosuccess != value)
                {
                  //  OnPropertyChanged(value, BCFUtility.getPropertyName(() => this.IsConnectinoSuccess));
                }
            }
        }
    }
}
