using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.UseCases.SaveMeeting
{
    public class SaveMeetingUseCaseTests
    {
        private ISaveEtraMeetingUseCase _classUnderTest;

        public SaveMeetingUseCaseTests()
        {
            _classUnderTest = new SaveEtraMeetingUseCase();
        }

        [Fact]
        public async Task calls_create_meeting_gateway()
        {

        }
    }
}
