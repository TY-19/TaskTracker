namespace TaskTracker.WebAPI.IntegrationTests.Controllers;

public class SubpartsTests
{


    [Fact]
    public async Task AssignmentController_GetAllSubpartsOfTheAssignment_ReturnsCorrectNumbersOfSubparts()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_GetAllSubpartsOfTheAssignment_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_GetSubpartById_ReturnsTheCorrectSubpart()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_GetSubpartById_ReturnsNotFound_IfSubpartDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_GetSubpartById_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_CreatesANewSubpart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_ReturnsBadRequestStatusCode_IfAssignmentIsIncorrect()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_ReturnsBadRequestStatusCode_IfModelContainDifferentAssignmentId()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_AddSubpartToTheAssignment_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task AssignmentController_UpdateSubpart_UpdatesSubpart()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_UpdateSubpart_ReturnsBadRequestStatusCode_IfSubpartDoesNotExist()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_UpdateSubpart_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_DeleteSubpart_DeletesSubpart()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_DeleteSubpart_DoesNotDeleteSubpartIfItDoesNotBelongToThisAssignment()
    {
        throw new NotImplementedException();
    }
    [Fact]
    public async Task AssignmentController_DeleteSubpart_ReturnsUnauthorizedStatusCode_IfUserIsNotAuthenticated()
    {
        throw new NotImplementedException();
    }
}
