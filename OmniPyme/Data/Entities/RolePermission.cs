namespace OmniPyme.Web.Data.Entities
{
    public class RolePermission
    {
        public int Roleid { get; set; }

        public PrivateURole Role { get; set; }


        public int permissionId  { get; set; }
        public Permission Permission { get; set; }
    }
}
