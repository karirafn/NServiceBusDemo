using System.Diagnostics;
using System.Net;

using OpenTelemetry;

namespace Shared;

internal class NetHostProcessor : BaseProcessor<Activity>
{
    private readonly string _hostName;

    public NetHostProcessor()
    {
        _hostName = Dns.GetHostName();
    }

    public override void OnStart(Activity data)
    {
        data.SetTag("net.host.name", _hostName);
    }
}
