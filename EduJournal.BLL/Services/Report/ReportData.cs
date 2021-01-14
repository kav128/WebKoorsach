using System;
using System.Linq;

namespace EduJournal.BLL.Services.Report
{
    /// <summary>
    /// Represents an academic attendance and attendance report.
    /// </summary>
    public record ReportData
    {
        /// <summary>
        /// Gets or inits report header.
        /// </summary>
        public ReportHeader Header { get; init; }
        
        /// <summary>
        /// Gets or inits an array of report records. 
        /// </summary>
        public ReportRecord[] Records { get; init; }
        
        /// <summary>
        /// Gets or inits report average score. Can be null.
        /// </summary>
        public double? AverageScore { get; init; }
        
        /// <summary>
        /// Gets or inits report attendance percentage. Can be null.
        /// </summary>
        public double? AttendancePercentage { get; init; }

        public virtual bool Equals(ReportData? other)
        {
            return other is not null &&
                   Header.Equals(other.Header) &&
                   Records.SequenceEqual(other.Records) &&
                   (AverageScore is not null && other.AverageScore is not null &&
                   Math.Abs(AverageScore.Value - other.AverageScore.Value) < 1E-06D ||
                   AverageScore is null && other.AverageScore is null) &&
                   (AttendancePercentage is not null && other.AttendancePercentage is not null &&
                    Math.Abs(AttendancePercentage.Value - other.AttendancePercentage.Value) < 1E-06D ||
                    AttendancePercentage is null && other.AttendancePercentage is null);
        }

        public override int GetHashCode() => HashCode.Combine(Header, Records, AverageScore, AttendancePercentage);
    }
}
