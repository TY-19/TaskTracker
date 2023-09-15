import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { SubpartService } from "./subpart.service";
import { environment } from "src/environments/environment";
import { Subpart } from "../models/subpart";
import { DummyDataHelper } from "src/tests/helpers/dummy-data-helper";

describe('SubpartService', () => {
    let service: SubpartService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule
        ],
        providers: [
          SubpartService
        ]
      })
      .compileComponents();

      service = TestBed.inject(SubpartService);
      httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

		it('getSubparts should call GET api/boards/:boardId/tasks/:taskId/subparts', () => {
      const boardId: number = 1;
			const taskId: number = 1;
      service.getSubparts(boardId, taskId).subscribe();

      const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId + "/subparts");
      expect(req.request.method).toBe('GET');
    });

		it('updateSubpart should call PUT api/boards/:boardId/tasks/:taskId/subparts/:subpartId', () => {
      const boardId: number = 1;
			const taskId: number = 1;
			const subpartId: number = 1;
			const subpart: Subpart = DummyDataHelper.getDummySubparts()[0];
      service.updateSubpart(boardId, taskId, subpartId, subpart).subscribe();

			const req = httpMock.expectOne(environment.baseUrl + 'api/boards/' + boardId + '/tasks/' + taskId + "/subparts/" + subpartId);
      expect(req.request.method).toBe('PUT');
    });
});