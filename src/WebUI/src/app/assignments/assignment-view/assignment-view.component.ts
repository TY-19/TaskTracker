import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Assignment } from 'src/app/models/assignment';
import { AssignmentService } from '../assignment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Stage } from 'src/app/models/stage';
import { StageService } from 'src/app/stages/stage.service';
import { Employee } from 'src/app/models/employee';
import { EmployeeService } from 'src/app/employees/employee.service';

@Component({
  selector: 'tt-assignment-view',
  templateUrl: './assignment-view.component.html',
  styleUrls: ['./assignment-view.component.scss']
})
export class AssignmentViewComponent implements OnInit {
  @Input() boardId?: string;
  @Input() assignmentId?: string;
  @Input() sidebarView: boolean = false;
  assignment?: Assignment;
  stage?: Stage;
  employee?: Employee;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private assignmentService: AssignmentService,
    private employeeService: EmployeeService,
    private stageService: StageService) { 

  }

  ngOnInit(): void {
    this.boardId ??= this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId ??= this.activatedRoute.snapshot.paramMap.get('taskId')!;
    this.getAssignment(this.boardId, this.assignmentId);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['assignmentId'] && !changes['assignmentId'].isFirstChange()) {
      this.assignmentId = changes['assignmentId'].currentValue;
      this.getAssignment(this.boardId ?? "0", this.assignmentId ?? "0");
    }
  }

  getAssignment(boardId: string, assignmentId: string) {
    this.assignmentService.getAssignment(boardId, assignmentId)
      .subscribe((result) => {
        this.assignment = result;      
        this.getStage(this.assignment.stageId);
        this.getEmployee(this.assignment.responsibleEmployeeId);
      });
  }

  getStage(stageId: number) {
    if (stageId) {
      this.stageService.getStage(this.boardId!, stageId.toString())
        .subscribe((result) => this.stage = result);
    }
  }

  getEmployee(employeeId: number) {
    if(employeeId) {
      this.employeeService.getEmployee(this.boardId!, employeeId)
        .subscribe(result => {
          this.employee = result;
        })
    }
  }

  deleteAssignment() {
    this.assignmentService.deleteAssignment(this.boardId!, this.assignmentId!)
      .subscribe(() => {
        this.router.navigate(['/boards', this.boardId])
          .catch(error => console.log(error))
      });
  }

  getAssignmentStatus(responseType: string = "text"): string {
    if(this.assignment?.isCompleted)
      return responseType == "class" ? "success-text" : "Completed";
    if(this.assignment?.deadline && new Date(this.assignment?.deadline) < new Date(Date.now()))
      return responseType == "class" ? "warn-text" : "Incompleted";
    return responseType == "class" ? "process-text" : "In progress";
  }


}
