import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { UserService } from "./user.service";
import { environment } from "src/environments/environment";
import { RegistrationRequest } from "../models/registration-request";
import { DummyDataHelper } from "src/tests/helpers/dummy-data-helper";
import { UserUpdateModel } from "../models/update-models/user-update-model";

describe('UserService', () => {
    let service: UserService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule
        ],
        providers: [
          UserService
        ]
      })
      .compileComponents();

      service = TestBed.inject(UserService);
      httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

		it('getUsers should call GET api/users', () => {
      service.getUsers().subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/users');
      expect(req.request.method).toBe('GET');
    });

		it('getUser should call GET api/users/:userName', () => {
      const userName = "TestUser";
			service.getUser(userName).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/users/' + userName);
      expect(req.request.method).toBe('GET');
    });

		it('addUser should call POST api/users', () => {
      const user: RegistrationRequest = DummyDataHelper.getDummyRegistrationRequest();
			service.addUser(user).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/users');
      expect(req.request.method).toBe('POST');
    });

		it('updateUser should call PUT api/users/:userName', () => {
      const userName = "TestUser";
			const toUpdate: UserUpdateModel = { firstName: "New first name" };
			service.updateUser(userName, toUpdate).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/users/' + userName);
      expect(req.request.method).toBe('PUT');
    });

		it('changePassword should call PUT api/users/:userName/changepassword', () => {
      const userName = "TestUser";
			const newPassword = "newPassword";
			service.changePassword(userName, newPassword).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/users/' + userName + '/changepassword');
      expect(req.request.method).toBe('PUT');
    });

		it('deleteUser should call DELETE api/users/:userName', () => {
      const userName = "TestUser";
			service.deleteUser(userName).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/users/' + userName);
      expect(req.request.method).toBe('DELETE');
    });
});