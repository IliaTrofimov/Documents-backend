using System.Text;

namespace Documents_backend.Models.Administrative
{
    public enum PermissionFlag
    {
        None = 0, 
        ReadDocuments = 0b1, WriteReadDocuments = 0b11,
        ReadTemplates = 0b100, WriteReadTemplates = 0b1100,
        EditDictionares = 0b10000,
        FullAccess = 0b11111,
    }

    public class UserPermission
    {
        public static byte Set(params PermissionFlag[] flags)
        {
            byte permission = 0;
            for (int i = 0; i < flags.Length && i < 6; i++)
                permission |= (byte)flags[i];
            return permission;
        }

        public static bool Has(byte permission, PermissionFlag flag)
        {
            return (permission & (byte)flag) == 0;
        }

        public static bool Has(byte permission, params PermissionFlag[] flags)
        {
            for (int i = 0; i < flags.Length && i < 6; i++)
                if ((permission & (byte)flags[i]) == 0) return false;
            return true;
        }

        public static string PermissionString(byte permission)
        {
            if (permission == 0)
                return "нет прав";

            string str = ""; 

            if (Has(permission, PermissionFlag.WriteReadDocuments))
                str += "чтение/редактирование документов, ";
            else if (Has(permission, PermissionFlag.ReadDocuments))
                str += "чтение документов, ";

            if (Has(permission, PermissionFlag.WriteReadTemplates))
                str += "чтение/редактирование шаблонов, ";
            else if (Has(permission, PermissionFlag.ReadTemplates))
                str += "чтение шаблонов, ";

            if (Has(permission, PermissionFlag.EditDictionares))
                str += "редактирование вспомогательной информации, ";

            return str;
        }
    }
}