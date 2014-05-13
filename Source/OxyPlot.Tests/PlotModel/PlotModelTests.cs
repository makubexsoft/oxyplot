﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotModelTests.cs" company="OxyPlot">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 OxyPlot contributors
//   
//   Permission is hereby granted, free of charge, to any person obtaining a
//   copy of this software and associated documentation files (the
//   "Software"), to deal in the Software without restriction, including
//   without limitation the rights to use, copy, modify, merge, publish,
//   distribute, sublicense, and/or sell copies of the Software, and to
//   permit persons to whom the Software is furnished to do so, subject to
//   the following conditions:
//   
//   The above copyright notice and this permission notice shall be included
//   in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//   OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//   IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//   CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//   TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//   SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Tests the <see cref="PlotModel.Axes" /> collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using ExampleLibrary;

    using NSubstitute;

    using NUnit.Framework;

    using OxyPlot.Axes;
    using OxyPlot.Series;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class PlotModelTests
    {
        [Test]
        public void Update_AllExamples_ThrowsNoExceptions()
        {
            foreach (var example in Examples.GetList())
            {
                example.PlotModel.Update();
            }
        }

        [Test]
        public void B11_Backgrounds()
        {
            var plot = new PlotModel { Title = "Backgrounds" };
            plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X-axis" });
            var yaxis1 = new LinearAxis { Position = AxisPosition.Left, Title = "Y1", Key = "Y1", StartPosition = 0, EndPosition = 0.5 };
            var yaxis2 = new LinearAxis { Position = AxisPosition.Left, Title = "Y2", Key = "Y2", StartPosition = 0.5, EndPosition = 1 };
            plot.Axes.Add(yaxis1);
            plot.Axes.Add(yaxis2);

            Action<LineSeries> addExamplePoints = ls =>
                {
                    ls.Points.Add(new DataPoint(3, 13));
                    ls.Points.Add(new DataPoint(10, 47));
                    ls.Points.Add(new DataPoint(30, 23));
                    ls.Points.Add(new DataPoint(40, 65));
                    ls.Points.Add(new DataPoint(80, 10));
                };

            var ls1 = new LineSeries { Background = OxyColors.LightSeaGreen, YAxisKey = "Y1" };
            addExamplePoints(ls1);
            plot.Series.Add(ls1);

            var ls2 = new LineSeries { Background = OxyColors.LightSkyBlue, YAxisKey = "Y2" };
            addExamplePoints(ls2);
            plot.Series.Add(ls2);

            // OxyAssert.AreEqual(plot, "B11");
        }

        [Test]
        public void PlotControl_CollectedPlotControl_ReferenceShouldNotBeAlive()
        {
            var plot = Substitute.For<IPlotView>();
            var pm = new PlotModel();
            pm.AttachPlotControl(plot);
            Assert.IsNotNull(pm.PlotControl);

            // ReSharper disable once RedundantAssignment
            plot = null;
            GC.Collect();

            // Verify that the reference is lost
            Assert.IsNull(pm.PlotControl);
        }

        /// <summary>
        /// Tests the <see cref="PlotModel.Render" /> method.
        /// </summary>
        public class Render
        {
            /// <summary>
            /// Tests rendering on a collapsed output surface.
            /// </summary>
            [Test]
            public void Collapsed()
            {
                var model = new PlotModel();
                var rc = Substitute.For<IRenderContext>();
                model.Render(rc, 0, 0);
            }

            /// <summary>
            /// Tests rendering on a small output surface.
            /// </summary>
            [Test]
            public void NoPadding()
            {
                var model = new PlotModel { Padding = new OxyThickness(0) };
                var rc = Substitute.For<IRenderContext>();
                model.Render(rc, double.Epsilon, double.Epsilon);
            }
        }

        /// <summary>
        /// Tests the <see cref="PlotModel.Axes" /> collection.
        /// </summary>
        public class Axes
        {
            /// <summary>
            /// The same axis cannot be added more than once.
            /// </summary>
            [Test]
            public void AddAxisTwice()
            {
                var model = new PlotModel();
                var axis = new LinearAxis();
                model.Axes.Add(axis);
                Assert.Throws<InvalidOperationException>(() => model.Axes.Add(axis));
            }

            /// <summary>
            /// The same axis cannot be added to different PlotModels.
            /// </summary>
            [Test]
            public void AddAxisToDifferentModels()
            {
                var model1 = new PlotModel();
                var model2 = new PlotModel();
                var axis = new LinearAxis();
                model1.Axes.Add(axis);
                Assert.Throws<InvalidOperationException>(() => model2.Axes.Add(axis));
            }
        }
    }
}