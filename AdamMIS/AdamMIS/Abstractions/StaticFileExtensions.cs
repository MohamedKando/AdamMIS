using Microsoft.Extensions.FileProviders;

namespace AdamMIS.Abstractions
{
    public static class StaticFileExtensions
    {
        public static void UseStaticFileMappings(this IApplicationBuilder app, Dictionary<string, string> mappings)
        {
            foreach (var mapping in mappings)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(mapping.Value),
                    RequestPath = mapping.Key
                });
            }
        }
    }

}
