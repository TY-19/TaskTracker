import { HttpEventType, HttpHeaders, HttpResponse } from "@angular/common/http";
import { ChangePasswordModel } from "src/app/models/update-models/change-password.model";
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
}