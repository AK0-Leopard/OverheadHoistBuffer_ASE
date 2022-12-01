using Mirle.Protos.ReserveModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.Expansion
{
    public static class HltDirectionExpansion
    {
        public static Direction convert2Direction(this Mirle.Hlts.Utils.HltDirection hltDirection)
        {
            switch (hltDirection)
            {
                case Mirle.Hlts.Utils.HltDirection.None:
                    return Direction.None;
                case Mirle.Hlts.Utils.HltDirection.Forward:
                    return Direction.Forward;
                case Mirle.Hlts.Utils.HltDirection.Reverse:
                    return Direction.Reverse;
                case Mirle.Hlts.Utils.HltDirection.Left:
                    return Direction.Left;
                case Mirle.Hlts.Utils.HltDirection.Right:
                    return Direction.Right;
                case Mirle.Hlts.Utils.HltDirection.ForwardReverse:
                    return Direction.ForwardReverse;
                case Mirle.Hlts.Utils.HltDirection.LeftRight:
                    return Direction.LeftRight;
                case Mirle.Hlts.Utils.HltDirection.FRLR:
                    return Direction.Frlr;
                case Mirle.Hlts.Utils.HltDirection.North:
                    return Direction.North;
                case Mirle.Hlts.Utils.HltDirection.East:
                    return Direction.East;
                case Mirle.Hlts.Utils.HltDirection.South:
                    return Direction.South;
                case Mirle.Hlts.Utils.HltDirection.West:
                    return Direction.West;
                case Mirle.Hlts.Utils.HltDirection.NorthSouth:
                    return Direction.NorthSouth;
                case Mirle.Hlts.Utils.HltDirection.EastWest:
                    return Direction.EastWest;
                case Mirle.Hlts.Utils.HltDirection.NESW:
                    return Direction.Nesw;
                default:
                    throw new Exception($"gRPC的HltDirection列舉:{hltDirection}，並無對應到使用的列舉");
            }

        }
    }
}