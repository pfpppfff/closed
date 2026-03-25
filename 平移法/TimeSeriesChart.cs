using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace 平移法
{
    public class TimeSeriesChart : Chart
    {
        private int maxPoints = 20; // Default maximum number of points to display

        public TimeSeriesChart()
        {
            //this.Dock = DockStyle.Fill;
            this.ChartAreas.Add(new ChartArea("MainArea"));
            this.Series.Clear(); // Ensure no default series are present
        }

        /// <summary>
        /// Adds a new series to the chart with the specified name and chart type.
        /// </summary>
        /// <param name="name">The name of the series.</param>
        /// <param name="chartType">The type of the chart (e.g., Line, Column).</param>
        public void AddSeries(string name, SeriesChartType chartType)
        {
            if (!this.Series.Any(s => s.Name == name))
            {
                var series = new Series(name) { ChartType = chartType };
                series.IsXValueIndexed = true;
                series.XValueType = ChartValueType.DateTime; // Assuming X values represent time
                this.Series.Add(series);
            }
        }

        /// <summary>
        /// Sets the maximum number of points to display for each series.
        /// </summary>
        /// <param name="maxPoints">The maximum number of points.</param>
        public void SetMaxPoints(int maxPoints)
        {
            this.maxPoints = maxPoints;
            foreach (var series in this.Series)
            {
                while (series.Points.Count > maxPoints)
                {
                    series.Points.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Adds a data point to the specified series.
        /// </summary>
        /// <param name="seriesName">The name of the series.</param>
        /// <param name="x">The X value of the data point (e.g., DateTime).</param>
        /// <param name="y">The Y value of the data point.</param>
        public void AddDataPoint(string seriesName, DateTime x, double y)
        {
            var series = this.Series.FirstOrDefault(s => s.Name == seriesName);
            if (series != null)
            {
                series.Points.AddXY(x, y);

                // Ensure that only the latest maxPoints are displayed
                while (series.Points.Count > maxPoints)
                {
                    series.Points.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Clears all data points from the specified series.
        /// </summary>
        /// <param name="seriesName">The name of the series to clear.</param>
        public void ClearSeries(string seriesName)
        {
            var series = this.Series.FirstOrDefault(s => s.Name == seriesName);
            if (series != null)
            {
                series.Points.Clear();
            }
        }

        /// <summary>
        /// Clears all series from the chart.
        /// </summary>
        public void ClearAllSeries()
        {
            this.Series.Clear();
        }
    }

}
