using System;
using System.Collections.Generic;
using System.Text;
using BCC.Core.CNB;
using BCC.Core;
using System.Linq;
using Xunit;
using Moq;

namespace BCC.Test
{
    public class CNBUnitTest
    {
       
      

        public static TheoryData<DateTime, DateTime, int[]> Data
        {
            get
            {
                var data =new TheoryData<DateTime, DateTime, int[]>();
                data.Add(new DateTime(2016,1,1), new DateTime(2019, 1, 1), new int[] { 2016,2017,2018,2019});
                return data;
            }
        }

        [Theory]
        [MemberData(memberName: nameof(Data))]
        public void TestYearInterval(DateTime start ,DateTime end, int[] expected) {

            List<int> expectedResult = expected.ToList();
            expectedResult.Sort();

            List<int> result = CNBank.YearsInterval(start, end);
            result.Sort();

            Assert.Equal(expectedResult, result);


        }

        [Fact]
        public void TestFoo()
        {
            var mock = new Mock<IWebClient>();
            mock.Setup(foo => foo.DownloadString("url")).Returns("abc");
            CNBank cnbBank =  new CNBank();
            cnbBank.BankWebClient = new MockWebClient();

            Assert.Throws<BCCCoreException>(()=>cnbBank.DownloadTodaysTicket());
        }
    }
    public interface IFoo
    {
        string DoSomething(string input);
    }

    public class Bar: IFoo
    {
        public string DoSomething(string input)
        {
            return input;
        }
    }
}
