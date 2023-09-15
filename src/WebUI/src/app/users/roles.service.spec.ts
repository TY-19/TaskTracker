import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { RolesService } from "./roles.service";
import { environment } from "src/environments/environment";
import { DefaultRolesNames } from "../config/default-roles-names";

describe('RolesService', () => {
    let service: RolesService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule
        ],
        providers: [
          RolesService
        ]
      })
      .compileComponents();

      service = TestBed.inject(RolesService);
      httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

		it('getAllRoles should call GET api/users/roles', () => {
      service.getAllRoles().subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/users/roles');
      expect(req.request.method).toBe('GET');
    });

		it('isAdmin should return true if user is admin', () => {
			const roles = [
				DefaultRolesNames.DEFAULT_ADMIN_ROLE,
				DefaultRolesNames.DEFAULT_MANAGER_ROLE,
				DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
			];
      const result = service.isAdmin(roles)

			expect(result).toEqual(true);
    });

		it('isAdmin should return false if user is not admin', () => {
      const roles = [
				DefaultRolesNames.DEFAULT_MANAGER_ROLE,
				DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
			];
      const result = service.isAdmin(roles)

			expect(result).toEqual(false);
    });

		it('isManager should return true if user is manager', () => {
      const roles = [
				DefaultRolesNames.DEFAULT_ADMIN_ROLE,
				DefaultRolesNames.DEFAULT_MANAGER_ROLE,
				DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
			];
      const result = service.isManager(roles)

			expect(result).toEqual(true);
    });

		it('isManager should return true if user is not manager', () => {
      const roles = [
				DefaultRolesNames.DEFAULT_ADMIN_ROLE,
				DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
			];
      const result = service.isManager(roles)

			expect(result).toEqual(false);
    });

		it('isEmployee should return true if user is employee', () => {
      const roles = [
				DefaultRolesNames.DEFAULT_ADMIN_ROLE,
				DefaultRolesNames.DEFAULT_MANAGER_ROLE,
				DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE
			];
      const result = service.isEmployee(roles)

			expect(result).toEqual(true);
    });

		it('isEmployee should return true if user is not employee', () => {
      const roles = [
				DefaultRolesNames.DEFAULT_ADMIN_ROLE,
				DefaultRolesNames.DEFAULT_MANAGER_ROLE,
			];
      const result = service.isEmployee(roles)

			expect(result).toEqual(false);
    });
});