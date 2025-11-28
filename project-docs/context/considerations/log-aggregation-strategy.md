# Log Aggregation Strategy

## Purpose

Centralize logs from multiple sources for searching, alerting, and analysis. Critical for production debugging and observability.

## Why Aggregation Matters

**Without aggregation:**
- Logs scattered across multiple servers
- SSH into servers to tail log files
- No central search or filtering
- Difficult to correlate events across services

**With aggregation:**
- Single interface to search all logs
- Correlation by request ID across services
- Real-time alerting on errors/patterns
- Performance analysis and dashboards

## Technology Options

### Application Insights (Azure)
**Best for:** Azure-hosted apps, Microsoft ecosystem

**Pros:**
- Auto-instruments ASP.NET Core and EF Core
- Built-in dashboards and alerting
- Excellent .NET integration
- Application performance monitoring (APM) included

**Cons:**
- Azure-specific (vendor lock-in)
- Can get expensive at scale
- Less flexible than self-hosted

**Cost:** ~$2.30/GB ingested

### ELK Stack (Elasticsearch, Logstash, Kibana)
**Best for:** Self-hosted, full control, multi-cloud

**Pros:**
- Powerful search and analysis
- Highly customizable
- No vendor lock-in
- Great visualization with Kibana

**Cons:**
- Complex to set up and maintain
- Resource intensive (RAM hungry)
- Requires DevOps expertise

**Cost:** Infrastructure costs (servers, storage)

### Seq (Structured Logs)
**Best for:** .NET developers, structured logging, development/small teams

**Pros:**
- Built for structured logging
- Excellent .NET integration
- Easy to set up (single container)
- Great for development and small scale

**Cons:**
- Less mature than ELK/Splunk
- Smaller ecosystem
- Limited APM features

**Cost:** Free for development, ~$500/year for small teams, scales up

### DataDog / New Relic
**Best for:** Enterprise, comprehensive observability

**Pros:**
- Full observability (logs, metrics, traces, APM)
- Excellent dashboards and alerting
- Easy setup, managed service
- Great support

**Cons:**
- Expensive at scale
- Vendor lock-in

**Cost:** ~$15-100/host/month + data ingestion

### CloudWatch (AWS)
**Best for:** AWS-hosted apps

**Pros:**
- Native AWS integration
- Simple setup for AWS workloads
- Good retention options

**Cons:**
- AWS-specific
- Query language less powerful than competitors
- UI not as polished

**Cost:** ~$0.50/GB ingested + storage

## Current Implementation

**Serilog Configuration:**
- Already configured in `Program.cs`
- Currently logging to console and file
- Structured logging enabled (JSON format)

**Adding a sink:**
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log")
    .WriteTo.Seq("http://localhost:5341") // Add Seq
    // .WriteTo.ApplicationInsights(telemetryConfig) // Or App Insights
    // .WriteTo.Elasticsearch(elasticConfig) // Or Elasticsearch
    .CreateLogger();
```

## Recommended Path

**Phase 1: Development (Current)**
- ✅ File logging with Serilog
- ✅ Structured logging enabled
- Consider: Add Seq for local development (Docker container)

**Phase 2: Staging/Small Production**
- Seq hosted on a small VM or container
- ~$500/year for small team license
- Easy to set up, great for .NET

**Phase 3: Production Scale**
- **If Azure**: Application Insights (easiest, best integration)
- **If AWS**: CloudWatch Logs or ELK on EC2/EKS
- **If self-hosted**: ELK Stack or Seq at scale
- **If enterprise**: DataDog or New Relic (full observability)

## Key Features to Consider

**Must-have:**
- Full-text search across all logs
- Filtering by log level, timestamp, service
- Correlation ID support (trace requests across services)
- Alerting on patterns (error rates, slow requests)

**Nice-to-have:**
- Real-time log tailing
- Custom dashboards
- APM (distributed tracing)
- Log retention policies
- User access controls

## Integration Requirements

**Serilog sinks to install:**
- `Serilog.Sinks.Seq` - for Seq
- `Serilog.Sinks.ApplicationInsights` - for Azure
- `Serilog.Sinks.Elasticsearch` - for ELK
- `Serilog.Sinks.Datadog.Logs` - for DataDog

**Configuration:**
- Store connection strings in `appsettings.json`
- Use different sinks for different environments
- Consider buffering/batching for performance

## Data Retention

**Considerations:**
- How long to keep logs? (7 days? 30 days? 1 year?)
- Compliance requirements (audit logs may need 7 years)
- Storage costs increase over time
- Archive old logs to cheaper storage (S3, Azure Blob)

**Typical strategy:**
- Hot storage: 7-30 days (fast search)
- Warm storage: 30-90 days (slower search)
- Cold storage: 90+ days (archive, rare access)

## Open Questions

- Where will the app be hosted? (Azure/AWS/self-hosted)
- Expected log volume (GB per day)?
- Budget for logging infrastructure?
- Do we need APM/distributed tracing or just logs?
- Compliance requirements for log retention?
