using System;
using System.Collections.Generic;
using MCSharp.World;
namespace MCSharp
{
    public class Group
    {
        // Static Members
        public static List<Group> GroupList = new List<Group>();
        public static Group standard;


        // Properties
        public bool CanExecute(Command cmd) { return cmd.CanUse(groupLevel); }
        public bool CanChat { get { return blnChat; } }
        public CommandList commands { get { return null;  } }
        public Int32 CuboidLimit { get { return mCuboidLimit; } }
        public LevelPermission Permission { get { return lvlPermission; } }
        public string Name { get { return strName; } }
        public string Color { get { return strColor; } }
        
        
        private bool blnChat;
        private Int32 mCuboidLimit;
        private GroupEnum groupLevel;
        private LevelPermission lvlPermission;
        private string strName;
        private string strColor;
        
        // Constructor
        public Group(string name, string color, GroupEnum g, bool chat, LevelPermission permission, Int32 _cuboidLimit)
        {
            blnChat = chat;
            groupLevel = g;
            lvlPermission = permission;
            strColor = color;
            strName = name;
            mCuboidLimit = _cuboidLimit;
        }

        #region "Operator Overloads..."

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Group g = (Group)obj;
            return groupLevel == g.groupLevel;
        }

        public static bool operator ==(Group g1, Group g2)
        {
            return (g1.groupLevel == g2.groupLevel);
        }

        public static bool operator !=(Group g1, Group g2)
        {
            return (g1.groupLevel != g2.groupLevel);
        }

        public static bool operator >(Group g1, Group g2)
        {
            return (g1.groupLevel > g2.groupLevel);
        }

        public static bool operator <(Group g1, Group g2)
        {
            return (g1.groupLevel < g2.groupLevel);
        }


        /// <summary>
        /// Got rid of a warning. Might want to rethink how to return a hash code...
        /// </summary>
        /// <returns>GroupLevel for now</returns>
        public override int GetHashCode()
        {
            return (int)groupLevel;
        }
        #endregion

        public static void InitAll()
        {
            GroupList.Add(new Group("banned", "&7", GroupEnum.Banned, true, LevelPermission.Guest, -1));
            GroupList.Add(new Group("guest", "&7", GroupEnum.Guest, true, LevelPermission.Guest, -1));
            GroupList.Add(new Group("builder", "&a", GroupEnum.Builder, true, LevelPermission.Builder, -1));
            GroupList.Add(new Group("advbuilder", "&2", GroupEnum.AdvBuilder, true, LevelPermission.AdvBuilder, Properties.advBuilderCuboidLimit));
            GroupList.Add(new Group("moderator", "&3", GroupEnum.Moderator, true, LevelPermission.Moderator, Properties.moderatorCuboidLimit));
            GroupList.Add(new Group("operator", "&1", GroupEnum.Operator, true, LevelPermission.Operator, Properties.operatorCuboidLimit));
            GroupList.Add(new Group("administrator", "&4", GroupEnum.Administrator, true, LevelPermission.Admin, Properties.administratorCuboidLimit));
            GroupList.Add(new Group("bots", "[bot]&6", GroupEnum.Guest, true, LevelPermission.Guest, -1));

            standard = GroupList[1];
        }

        // Does a group exist?
        public static bool Exists(string name)
        {
            name = name.ToLower(); foreach (Group gr in GroupList)
            {
                if (gr.Name == name.ToLower()) { return true; }
            } return false;
        }

        // Find a group by string
        public static Group Find(string name)
        {
            Group group = null;
            name = name.ToLower(); foreach (Group gr in GroupList)
            {
                if (gr.Name == name.ToLower())
                {
                    group = gr;
                }
            }
            return group;
        }

        // Find a group by GroupEnum type
        public static Group Find(GroupEnum g)
        {
            Group group = null;
            foreach (Group gr in GroupList)
            {
                if (gr.groupLevel == g)
                {
                    group = gr;
                    break;
                }
            }
            return group;
        }
    }
}