namespace AdamMIS.Abstractions.Consts
{
    public static class ImpliedPermissions
    {
        public static readonly Dictionary<string, string[]> Map = new()
    {
        { Permissions.ViewAdminManager, new[] { Permissions.ReadUsers, Permissions.ReadRoles } },
        { Permissions.ViewReportManager, new[] { Permissions.ReadReports } }
    };

        public static IEnumerable<string> Expand(IEnumerable<string> permissions)
        {
            var expanded = new HashSet<string>(permissions);

            foreach (var perm in permissions)
            {
                if (Map.TryGetValue(perm, out var implied))
                {
                    foreach (var p in implied)
                        expanded.Add(p);
                }
            }

            return expanded;
        }
    }
}
