using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.ResolveRenderingDatasource;
using Sitecore.Text;
using Sitecore.XA.Foundation.Abstractions;
using Sitecore.XA.Foundation.Multisite.Extensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.TokenResolution;

namespace Sitecore.Support.XA.Foundation.LocalDatasources.Pipelines.ResolveRenderingDatasource
{
    public class QueryableDatasource
    {
        public void Process(ResolveRenderingDatasourceArgs args)
        {
            Assert.IsNotNull(args, "args");

            if (args.Datasource.StartsWith("query:", StringComparison.InvariantCultureIgnoreCase))
            {
                Item contextItem = args.GetContextItem();
                if (contextItem != null)
                {
                    if (!ServiceLocator.ServiceProvider.GetService<IContext>().Site.IsSxaSite())
                    {
                        return;
                    }

                    string query = TokenResolver.Resolve(args.Datasource, contextItem);
                    Item[] queryItems = contextItem.SelectItemsWithLanguage(query);
                    ListString itemIds = new ListString(queryItems.Select(i => i.ID.ToString()).ToList());
                    args.Datasource = itemIds.ToString();
                    return;
                }

                args.Datasource = string.Empty;
            }
        }
    }
}