import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Assignment } from 'src/app/models/assignment';
import { AssignmentService } from '../assignment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Stage } from 'src/app/models/stage';
import { StageService } from 'src/app/stages/stage.service';
import { Employee } from 'src/app/models/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { AuthService } from 'src/app/auth/auth.service';
import { AssignmentDisplayService } from '../assignment-display.service';

@Component({
  selector: 'tt-assignment-view',
  templateUrl: './assignment-view.component.html',
  styleUrls: ['./assignment-view.component.scss']
})
export class AssignmentViewComponent implements OnInit, OnChanges {
  @Input() boardId?: number | string;
  @Input() assignmentId?: number |string;
  @Input() sidebarView: boolean = false;
  assignment?: Assignment;
  stage?: Stage;
  employee?: Employee;

  constructor(private authService: AuthService,
    private assignmentService: AssignmentService,
    private assignmentDisplayService: AssignmentDisplayService,
    private employeeService: EmployeeService,
    private stageService: StageService,
    private activatedRoute: ActivatedRoute,
    private router: Router) {

  }

  ngOnInit(): void {
    this.boardId ??= this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId ??= this.activatedRoute.snapshot.paramMap.get('taskId')!;
    this.loadAssignment(this.boardId, this.assignmentId);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['assignmentId'] && !changes['assignmentId'].isFirstChange()) {
      this.assignmentId = changes['assignmentId'].currentValue;
      this.loadAssignment(this.boardId!, this.assignmentId!);
    }
  }

  loadAssignment(boardId: number | string, assignmentId: number | string): void {
    this.assignmentService.getAssignment(boardId, assignmentId)
      .subscribe((result) => {
        this.assignment = result;      
        this.loadStage(this.assignment.stageId);
        this.loadEmployee(this.assignment.responsibleEmployeeId);
      });
  }

  loadStage(stageId: number | string): void {
    this.stageService.getStage(this.boardId!, stageId)
      .subscribe(result => this.stage = result);
  }

  loadEmployee(employeeId: number | string): void {
    this.employeeService.getEmployee(this.boardId!, employeeId)
      .subscribe(result => this.employee = result);
  }

  changeAssignmentStatus(assignment: Assignment): void {
    this.assignmentService
      .changeAssignmentStatus(this.boardId!, assignment.id, !assignment.isCompleted)
        .subscribe(() => this.loadAssignment(this.boardId!, assignment.id));
  }

  deleteAssignment(): void {
    this.assignmentService.deleteAssignment(this.boardId!, this.assignmentId!)
      .subscribe(() => { this.router.navigate(['/boards', this.boardId]) });
  }

  get isAdminOrManager(): boolean {
    return this.authService.isAdmin() || this.authService.isManager();
  }

  get isUserAuthorizeToChangeTaskStatus(): boolean {
    return this.assignmentDisplayService.isUserAuthorizeToModifyTask(this.assignment!);
  }

  get assignmentStatus(): { class: string; text: string } {
    if(this.assignment?.isCompleted) {
      return { class: "success-text", text: "Completed"};
    } else if(this.assignment?.deadline
        && new Date(this.assignment?.deadline) < new Date(Date.now())) {
      return { class: "warn-text", text: "Incompleted"};
    } else {
      return { class: "process-text", text: "In progress"};
    }
  }
}
