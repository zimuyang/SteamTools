// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.DomainResolve/DomainResolver.cs

using System.Application.Models;
using System.Net;

namespace System.Application.Services.Implementation;

sealed class DomainResolver : IDomainResolver
{
    readonly IReverseProxyConfig reverseProxyConfig;

    bool isIpv6 = false;

    public DomainResolver(IReverseProxyConfig reverseProxyConfig)
    {
        this.reverseProxyConfig = reverseProxyConfig;
    }

    public IAsyncEnumerable<IPAddress> ResolveAsync(DnsEndPoint endPoint, CancellationToken cancellationToken = default)
    {
        try
        {
            if (IReverseProxyService.Instance.ProxyDNS != null)
            {
                return reverseProxyConfig.Service.DnsAnalysis.AnalysisDomainIpAsync(endPoint.Host, new IPAddress[] { IReverseProxyService.Instance.ProxyDNS }, isIpv6, cancellationToken);
            }
            else if (IReverseProxyService.Instance.ProxyMode is ProxyMode.DNSIntercept or ProxyMode.Hosts)
            {
                // hosts 加速下不能使用系统默认 DNS 解析代理，会解析到 hosts 或 DNS拦截器 上无限循环
                return reverseProxyConfig.Service.DnsAnalysis.AnalysisDomainIpAsync(endPoint.Host, IDnsAnalysisService.DNS_Dnspods, isIpv6, cancellationToken);
            }
            return reverseProxyConfig.Service.DnsAnalysis.AnalysisDomainIpAsync(endPoint.Host, isIpv6, cancellationToken);
        }
        catch (Exception ex)
        {
            INotificationService.Instance.Notify(UI.Resx.AppResources.CommunityFix_DNSErrorNotify + Environment.NewLine + "Exception Message :" + ex.Message, NotificationType.Message);
            throw;
        }
    }

    public async void CheckIpv6SupportAsync(CancellationToken cancellationToken = default)
    {
        isIpv6 = await reverseProxyConfig.Service.DnsAnalysis.GetIsIpv6Support();
    }

    public Task TestSpeedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
