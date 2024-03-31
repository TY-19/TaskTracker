import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AssignmentsComponent } from './assignments.component';
import { AuthService } from '../auth/auth.service';
import { AssignmentService } from './assignment.service';
import { AssignmentDisplayService } from './assignment-display.service';
import { BoardService } from '../boards/board.service';
import { ActivatedRoute, ParamMap, convertToParamMap } from '@angular/router';
import { Board } from '../models/board';
import { Subject } from 'rxjs';

describe('AssignmentsComponent', () => {
  let component: AssignmentsComponent;
  let fixture: ComponentFixture<AssignmentsComponent>;
  let authServiceMock: jasmine.SpyObj<AuthService>;
  let assignmentServiceMock: jasmine.SpyObj<AssignmentService>;
  let assignmentDisplayServiceMock: jasmine.SpyObj<AssignmentDisplayService>;
  let boardServiceMock: jasmine.SpyObj<BoardService>;
  let activatedRouteMock: {snapshot: { paramMap: ParamMap}};

  beforeEach(async () => {
    configureMocksWithDefaultSettings();
    
    await TestBed.configureTestingModule({
      declarations: [ AssignmentsComponent ],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: AssignmentService, useValue: assignmentServiceMock },
        { provide: AssignmentDisplayService, useValue: assignmentDisplayServiceMock },
        { provide: BoardService, useValue: boardServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  function configureMocksWithDefaultSettings() {
    authServiceMock = jasmine.createSpyObj('authService',
      ['isAdmin', 'isManager']);
    authServiceMock.isAdmin.and.returnValue(false);
    authServiceMock.isManager.and.returnValue(false);

    assignmentServiceMock = jasmine.createSpyObj('assignmentService', ['']);
    assignmentDisplayServiceMock = jasmine.createSpyObj('assignmentDisplayService', ['']);
    
    boardServiceMock = jasmine.createSpyObj('boardService',
      ['getBoard', 'getBoardsOfTheEmployee']);
    boardServiceMock.getBoard.and.returnValue(new Subject<Board>().asObservable());
    boardServiceMock.getBoardsOfTheEmployee.and.returnValue(new Subject<Board[]>().asObservable());

    activatedRouteMock = {
      snapshot: {
        paramMap: convertToParamMap({ boardId: '1' }),
      },
    };
  }

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set boardId during ngOnInit if it is presented in route', () => {
    expect(component.boardId).toBeTruthy();
  });

  it('should set boardId to null during ngOnInit if it is not presented in route', () => {
    activatedRouteMock.snapshot.paramMap = convertToParamMap({ });
    component.ngOnInit();
    
    expect(component.boardId).toBeFalsy();
  });

  it('should set correct mode during ngOnInit', () => {
    
  });

  it('should contain correct displayColumns in singleBoardMode', () => {

  });

  it('should contain correct displayColumns in multipleBoardsMode for manager', () => {

  });

  it('should contain correct displayColumns in multipleBoardsMode for employee', () => {

  });

  it('should load board in singleBoardMode', () => {

  });

  it('should load boards in multipleBoardsMode', () => {

  });

  it('filtering by topic should work', () => {

  });

  it('filtering by employee\'s name should work', () => {

  });

  it('clearFilter should work', () => {

  });

  it('selectEmployeesTasks should work', () => {

  });

  it('deleteAssignment should work', () => {

  });
});
