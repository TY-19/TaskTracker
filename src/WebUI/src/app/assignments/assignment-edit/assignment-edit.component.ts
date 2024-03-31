import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignmentService } from '../assignment.service';
import { StageService } from 'src/app/stages/stage.service';
import { Stage } from 'src/app/models/stage';
import { Employee } from 'src/app/models/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { Assignment } from 'src/app/models/assignment';
import { Subpart } from 'src/app/models/subpart';
import { SubpartsComponent } from 'src/app/subparts/subparts.component';
import { CustomValidators } from 'src/app/common/custom-validators';
import { DisplayModes } from 'src/app/common/enums/display-modes';
import { employeeByFirstNameComparator } from 'src/app/common/helpers/comparators';
import { AssignmentDisplayService } from '../assignment-display.service';

@Component({
  selector: 'tt-assignment-edit',
  templateUrl: './assignment-edit.component.html',
  styleUrls: ['./assignment-edit.component.scss']
})
export class AssignmentEditComponent implements OnInit {
  @Input() boardId?: number | string;
  @Input() assignmentId?: number | string;
  @Input() sidebarView: boolean = false;
  @ViewChild(SubpartsComponent) subpartsComponent!: SubpartsComponent;

  stages: Stage[] = [];
  subparts: Subpart[] = [];
  employees: Employee[] = [];

  form!: UntypedFormGroup;
  get isFormValid(): boolean {
    return this.subpartsComponent.areAllSubpartsValid() && this.form.valid;
  };
  mode: DisplayModes = DisplayModes.Edit;
  get isInCreateMode() {
    return this.mode === DisplayModes.Create;
  }
  
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private assignmentService: AssignmentService,
    private assignmentDisplayService: AssignmentDisplayService,
    private employeeService: EmployeeService,
    private stageService: StageService) { 
      
    }
  
  ngOnInit(): void {
    this.setFields();
    this.initiateForm();
    this.prepareData();
  }

  private setFields(): void {
    this.boardId ??= this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId ??= this.activatedRoute.snapshot.paramMap.get('taskId') ?? 0;
    this.mode = this.assignmentId == 0 ? DisplayModes.Create : DisplayModes.Edit;
  }

  private initiateForm(): void {
    this.form = new UntypedFormGroup({
      id: new UntypedFormControl(0, [
        Validators.required
      ]),
      topic: new UntypedFormControl("", [
        Validators.required,
        Validators.maxLength(50)
      ]),
      description: new UntypedFormControl(),
      stageId: new UntypedFormControl(null, [
        Validators.required
      ]),
      deadlineDate: new UntypedFormControl(new Date(), [
        Validators.required
      ]),
      deadlineTime: new UntypedFormControl("23:00"),
      responsibleEmployeeId: new UntypedFormControl("", [
        Validators.required
      ]),
      isCompleted: new UntypedFormControl(false)
    }, {
      validators: CustomValidators.dateInTheFutureValidator()
    });
  }

  private prepareData(): void {
    this.loadStages();
    this.loadEmployees();
    if (this.mode === DisplayModes.Edit) {
      this.loadAssignment();
    }
    else {
      this.subparts = [];
    }
  }

  private loadStages(): void {
    this.stageService.getStages(this.boardId!)
      .subscribe(stages => {
        this.stages = stages;
      });
  }

  private loadEmployees(): void {
    this.employeeService.getEmployees(this.boardId!)
      .subscribe(result => {
        this.employees = result;
        this.employees.sort(employeeByFirstNameComparator);
      });
  }

  private loadAssignment(): void {
    this.assignmentService.getAssignment(this.boardId!, this.assignmentId!)
      .subscribe(result => {
        this.patchForm(result);
        this.subparts = result.subparts;
      });
  }

  private patchForm(assignment: Assignment): void {
    this.form.patchValue(assignment);
    this.form.patchValue({ 'deadlineDate': assignment.deadline });
    this.form.patchValue({ 'deadlineTime':
      this.assignmentDisplayService.getTimeFromDateTime(assignment.deadline) });
  }

  updateStage(): void {
    this.assignmentService.getAssignment(this.boardId!, this.assignmentId!)
      .subscribe(result => this.form.patchValue({ 'stageId': result.stageId }));
  }

  onSubmit(): void {
    if(this.isFormValid)
    {
      const assignment = this.getAssignmentFromTheForm();
      
      if (this.mode === DisplayModes.Create)
        this.createAssignment(assignment);
      else if (this.mode === DisplayModes.Edit)
        this.updateAssignment(assignment);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private getAssignmentFromTheForm(): Assignment {
    return {
      id: this.form.controls['id'].value,
      topic: this.form.controls['topic'].value,
      description: this.form.controls['description'].value,
      boardId: Number(this.boardId),
      stageId: this.form.controls['stageId'].value,
      deadline: this.getDeadline(),
      responsibleEmployeeId: this.form.controls['responsibleEmployeeId'].value,
      isCompleted: this.form.controls['isCompleted'].value,
      subparts: this.subpartsComponent.getSubparts()
    };
  }

  private getDeadline() : Date {
      const deadlineDate = this.form.controls['deadlineDate'].value;
      const deadlineTime = this.form.controls['deadlineTime'].value;
      return this.assignmentDisplayService.getDeadline(deadlineDate, deadlineTime);
  }

  private createAssignment(assignment: Assignment): void {
    this.assignmentService.createAssignment(this.boardId!, assignment)
      .subscribe( () => {
        if(!this.sidebarView) {
          this.router.navigate(['/boards', this.boardId]);
        }
    });
  }

  private updateAssignment(assignment: Assignment): void {
    this.assignmentService.updateAssignment(this.boardId!, assignment)
      .subscribe( () => {
        if(!this.sidebarView) {
          this.router.navigate(['/boards', this.boardId, 'tasks', this.assignmentId]);
        }
      });
  }
}
