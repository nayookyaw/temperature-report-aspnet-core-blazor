using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BackendAspNetCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAspNetCore.Controllers
{
    [ApiController]
    [Route("health")] // global "api" prefix will make it /api/health/...
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HealthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Live / Startup probe: is the process running?
        /// Light-weight check, no heavy external deps.
        /// GET /api/health/live
        /// </summary>
        [HttpGet("live")]
        public IActionResult Live()
        {
            return Ok(new
            {
                status = "Live",
                timestampUtc = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Ready / Liveness probe: can we actually serve traffic?
        /// You can plug in CPU, queue length, DB connectivity, etc.
        /// GET /api/health/ready
        /// </summary>
        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            // 🔹 Example: DB connectivity check
            var canConnectDb = await _dbContext.Database.CanConnectAsync();

            if (!canConnectDb)
            {
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    reason = "Database unreachable",
                    timestampUtc = DateTime.UtcNow
                });
            }

            // 🔹 OPTIONAL: add CPU / queue checks here
            var cpuUsage = GetCpuPercent();
            // var queueLength = GetBackgroundQueueLength();
            
            if (cpuUsage > 85)
            {
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    reason = "High CPU or queue length",
                    cpuUsage,
                    timestampUtc = DateTime.UtcNow
                });
            }

            return Ok(new
            {
                status = "Ready",
                timestampUtc = DateTime.UtcNow
            });
        }

        // Example stub for CPU usage – you can replace with your own logic
        private static double GetCpuPercent()
        {
            try
            {
                // CPU usage information from cgroup v2
                var cpuStatPath = "/sys/fs/cgroup/cpu.stat";
                if (!System.IO.File.Exists(cpuStatPath))
                    return -1;

                var lines = System.IO.File.ReadAllLines(cpuStatPath);
                long usageUsec = 0;
                foreach (var line in lines)
                {
                    if (line.StartsWith("usage_usec"))
                    {
                        usageUsec = long.Parse(line.Split(' ')[1]);
                    }
                }
                // Convert usec → seconds
                double cpuSeconds = usageUsec / 1_000_000.0;
                // 🧠 NOTE:
                // cpuSeconds is total CPU consumed since container start,
                // not a "percentage". We need a sampling technique.

                return Math.Round(GetCpuDeltaPercent(cpuSeconds), 2);
            }
            catch
            {
                return -1;
            }
        }

        // ----------------------
        // Helper for CPU sampling
        // ----------------------
        private static double _lastCpuSeconds = 0;
        private static DateTime _lastCpuSample = DateTime.UtcNow;

        private static double GetCpuDeltaPercent(double currentCpuSeconds)
        {
            var now = DateTime.UtcNow;
            var elapsed = (now - _lastCpuSample).TotalSeconds;

            if (elapsed <= 0)
                return 0;

            // CPU delta
            double delta = currentCpuSeconds - _lastCpuSeconds;
            // # of CPUs assigned to the container (limit)
            double cpuLimit = GetCpuLimit();
            // CPU %
            double cpuPercent = (delta / elapsed) * 100.0 / cpuLimit;

            // Update tracking
            _lastCpuSeconds = currentCpuSeconds;
            _lastCpuSample = now;

            return cpuPercent < 0 ? 0 : cpuPercent;
        }

        private static double GetCpuLimit()
        {
            try
            {
                var quotaPath = "/sys/fs/cgroup/cpu.max"; 
                if (!System.IO.File.Exists(quotaPath)) return Environment.ProcessorCount;

                var parts = System.IO.File.ReadAllText(quotaPath).Split(' ');
                long quota = long.Parse(parts[0]);
                long period = long.Parse(parts[1]);

                if (quota < 0) return Environment.ProcessorCount; // unlimited

                return (double)quota / period;
            }
            catch
            {
                return Environment.ProcessorCount;
            }
        }

    }
}