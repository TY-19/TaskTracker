import { HttpEventType, HttpHeaders, HttpResponse } from "@angular/common/http";
import { Assignment } from "src/app/models/assignment";
import { Board } from "src/app/models/board";
import { Employee } from "src/app/models/employee";
import { RegistrationRequest } from "src/app/models/registration-request";
import { Stage } from "src/app/models/stage";
import { Subpart } from "src/app/models/subpart";
import { UserProfile } from "src/app/models/user-profile";

export class DummyDataHelper {
  static getDummyNoContentResponse(): HttpResponse<Object> {
    return {
      body: null,
      type: HttpEventType.Response,
      clone: function (): HttpResponse<Object> {
          throw new Error("Function not implemented.");
      },
      headers: new HttpHeaders,
      status: 204,
      statusText: "No content",
      url: null,
      ok: true
    };
  }

  static getDummyUserProfile(): UserProfile {
    return {
      id: '1',
      userName: 'TestUser',
      email: 'testUser@example.com',
      roles: [
        'Employee'
      ],
      firstName: 'Test',
      lastName: 'User',
      boardsIds: [
        1, 2
      ],
      assignmentsIds: [
        1, 6, 7
      ]
    };
  }

  static getDummyChangePasswordModel() {
    return {
      oldPassword: "oldPassword",
      password: "newPassword",
      passwordConfirm: "newPassword"
    }
  }

  static getDummyBoards(): Board[] {
    const stages: Stage[] = this.getDummyStages();
    const assignments: Assignment[] = this.getDummyAssignments();
    const employees: Employee[] = this.getDummyEmployees();
    return [
      {
        id: 1,
        name: "Board 1",
        stages: [stages[0], stages[1]],
        employees: [employees[0], employees[1]],
        assignments: [assignments[0], assignments[1], assignments[2]]
      },
      {
        id: 2,
        name: "Board 2",
        stages: [stages[2], stages[3]],
        employees: [employees[0], employees[1]],
        assignments: [assignments[3], assignments[4]]
      }
    ];
  }

  static getDummyAssignments(): Assignment[] {
    return [
      { id: 1, topic: "Topic 1", boardId: 1, stageId: 1, responsibleEmployeeId: 1, subparts: this.getDummySubparts() },
      { id: 2, topic: "Topic 2", boardId: 1, stageId: 2, responsibleEmployeeId: 1, subparts: [] },
      { id: 3, topic: "Topic 3", boardId: 1, stageId: 2, responsibleEmployeeId: 2, subparts: [] },
      { id: 4, topic: "Topic 4", boardId: 2, stageId: 1, responsibleEmployeeId: 1, subparts: [] },
      { id: 5, topic: "Topic 5", boardId: 2, stageId: 2, responsibleEmployeeId: 2, subparts: [] }
    ];
  }

  static getDummySubparts(): Subpart[] {
    return [
      { id: 1, name: "Subpart 1", percentValue: 50, isCompleted: false, assignmentId: 1 },
      { id: 2, name: "Subpart 2", percentValue: 50, isCompleted: false, assignmentId: 1 }
    ]
  }

  static getDummyStages(): Stage[] {
    return [
      { id: 1, name: "First", position: 1, boardId: 1 },
      { id: 2, name: "Second", position: 2, boardId: 1 },
      { id: 3, name: "First", position: 1, boardId: 2 },
      { id: 4, name: "Second", position: 2, boardId: 2 }
    ];
  }

  static getDummyEmployees(): Employee[] {
    return [
      { id: 1, userName: "FirstUser", roles: ["Employee"], firstName: "Test1", lastName: "Last1" },
      { id: 2, userName: "SecondUser", roles: ["Manager"], firstName: "Test2", lastName: "Last2" },
      { id: 3, userName: "ThirdUser", roles: ["Administrator"], firstName: "Test3", lastName: "Last3" }
    ];
  }

  static getDummyRegistrationRequest(): RegistrationRequest {
    return {
      userName: "TestUser",
      email: "testUser@example.com",
      password: "password"
    }
  }
}
