import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { EmployeeService } from "./employee.service";
import { environment } from "src/environments/environment";
import { Employee } from "../models/employee";
import { DummyDataHelper } from "src/tests/helpers/dummy-data-helper";

describe('EmployeeService', () => {
    let service: EmployeeService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule
        ],
        providers: [
          EmployeeService
        ]
      })
      .compileComponents();

      service = TestBed.inject(EmployeeService);
      httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

		it('getAllEmployees should call GET api/employees', () => {
      service.getAllEmployees().subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/employees');
      expect(req.request.method).toBe('GET');
    });

		it('getEmployees should call GET api/boards/:boardId/employees', () => {
      const boardId = 1;
			service.getEmployees(boardId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/employees');
      expect(req.request.method).toBe('GET');
    });

		it('getEmployee should call GET api/boards/:boardId/employees/:employeeId', () => {
      const boardId = 1;
			const employeeId = 1
			service.getEmployee(boardId, employeeId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/employees/' + employeeId);
      expect(req.request.method).toBe('GET');
    });

		it('addEmployeeToTheBoard should call POST api/boards/:boardId/employees/:username', () => {
      const boardId = 1;
			const username = "TestUser";
			service.addEmployeeToTheBoard(1, username).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/employees/' + username);
      expect(req.request.method).toBe('POST');
    });

		it('removeEmployeeFromTheBoard should call DELETE should call GET api/boards/:boardId/employees/:employeeId', () => {
      const boardId = 1;
			const employeeId = 1
			service.removeEmployeeFromTheBoard(boardId, employeeId).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/employees/' + employeeId);
      expect(req.request.method).toBe('DELETE');
    });

		it('filterEmployees should filter employees by username', () => {
      const filter = "SecondUser";
			const employees: Employee[] = DummyDataHelper.getDummyEmployees();
			const result = service.filterEmployees(filter, employees);
			expect(result.length).toEqual(1);
			expect(result[0]).toEqual(employees[1]);
    });

		it('filterEmployees should filter employees by first name', () => {
      const filter = "Manager";
			const employees: Employee[] = DummyDataHelper.getDummyEmployees();
			const result = service.filterEmployees(filter, employees);
			expect(result.length).toEqual(1);
			expect(result[0]).toEqual(employees[1]);
    });

		it('filterEmployees should filter employees by last name', () => {
      const filter = "Test2";
			const employees: Employee[] = DummyDataHelper.getDummyEmployees();
			const result = service.filterEmployees(filter, employees);
			expect(result.length).toEqual(1);
			expect(result[0]).toEqual(employees[1]);
    });

		it('filterEmployees should filter employees by role', () => {
      const filter = "Last2";
			const employees: Employee[] = DummyDataHelper.getDummyEmployees();
			const result = service.filterEmployees(filter, employees);
			expect(result.length).toEqual(1);
			expect(result[0]).toEqual(employees[1]);
    });
});