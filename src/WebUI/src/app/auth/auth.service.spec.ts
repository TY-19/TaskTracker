import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { AuthService } from "./auth.service";
import { DummyDataHelper } from "src/tests/helpers/dummy-data-helper";
import { LoginRequest } from "../models/login-request";
import { environment } from "src/environments/environment";
import { RegistrationRequest } from "../models/registration-request";

describe('AuthService', () => {
	let service: AuthService;
	let httpMock: HttpTestingController;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule
      ],
      providers: [
        AuthService
      ]
    })
    .compileComponents();

    service = TestBed.inject(AuthService);
	httpMock = TestBed.inject(HttpTestingController);
  });

	afterEach(() => {
		httpMock.verify();
	});

	it('should be created', () => {
    expect(service).toBeTruthy();
  });

	it('registration should call POST api/Account/registration', () => {
		const registrationRequest: RegistrationRequest = DummyDataHelper.getDummyRegistrationRequest();
		service.registration(registrationRequest).subscribe();
		
		const req = httpMock.expectOne(environment.baseUrl + "api/Account/registration");
		expect(req.request.method).toBe('POST');
		httpMock.verify();
	});
  
	it('login should work correctly if get successful response', () => {
		let status: boolean = false;
		let userName: string | null = null;
		service.authStatus.subscribe(newValue => status = newValue);
		service.userName.subscribe(newValue => userName = newValue);

		const loginRequest: LoginRequest = DummyDataHelper.getDummyLoginRequest();
		service.login(loginRequest).subscribe(response => {
			expect(response.success).toBeTrue();
			expect(response.token).toBeTruthy();
			expect(status).toBeTrue();
			expect(userName).toBe("TestUser");
			expect(service.getRoles().length > 0);
			expect(localStorage.getItem(service.tokenKey)).toBeTruthy();
		});

		const req = httpMock.expectOne(environment.baseUrl + "api/Account/login");
		req.flush(DummyDataHelper.getDummySuccessLoginResult());
		expect(req.request.method).toBe('POST');
	});

	it('login should work correctly if get unsuccessful response', () => {
		let status: boolean = false;
		let userName: string | null = null;
		service.authStatus.subscribe(newValue => status = newValue);
		service.userName.subscribe(newValue => userName = newValue);

		const loginRequest: LoginRequest = DummyDataHelper.getDummyLoginRequest();
		service.login(loginRequest).subscribe(response => {
			expect(response.success).toBeFalse();
			expect(response.token).toBeFalsy();
			expect(status).toBeFalse();
			expect(userName).toBeFalsy();
			expect(service.getRoles().length === 0);
		});

		const req = httpMock.expectOne(environment.baseUrl + "api/Account/login");
		req.flush(DummyDataHelper.getDummyFailLoginResult());
		expect(req.request.method).toBe('POST');
	});

	it('should correctly provide user information', () => {
		const loginRequest: LoginRequest = DummyDataHelper.getDummyLoginRequest();
		service.login(loginRequest).subscribe();

		expect(service.getToken()).toBeTruthy();
		expect(service.getRoles().length).toBeGreaterThan(0);
		expect(service.getUserName()).toBeTruthy();
		expect(service.getEmployeeId()).toBeTruthy();
		expect(service.isAuthenticated()).toBeTrue();
		expect(service.isAdmin()).toBeFalse();
		expect(service.isManager()).toBeFalse();
		expect(service.isEmployee()).toBeTrue();

		const req = httpMock.expectOne(environment.baseUrl + "api/Account/login");
		req.flush(DummyDataHelper.getDummySuccessLoginResult());
	});

	it('logout should work correctly', () => {
		const loginRequest: LoginRequest = DummyDataHelper.getDummyLoginRequest();
		let status: boolean = false;
		let userName: string | null = null;
		service.authStatus.subscribe(newValue => status = newValue);
		service.userName.subscribe(newValue => userName = newValue);

		service.login(loginRequest).subscribe(() => {
			service.logout();
			expect(status).toBeFalsy();
			expect(userName).toBeFalsy();
			expect(service.getToken()).toBeFalsy();
			expect(service.getRoles().length).toBe(0);
			expect(service.getUserName()).toBeFalsy();
			expect(service.getEmployeeId()).toBeFalsy();
			expect(service.isAuthenticated()).toBeFalse();
		});	

		const req = httpMock.expectOne(environment.baseUrl + "api/Account/login");
		req.flush(DummyDataHelper.getDummySuccessLoginResult());
	});
});
