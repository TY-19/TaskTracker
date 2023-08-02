import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignmentService } from '../assignment.service';
import { StageService } from 'src/app/stages/stage.service';
import { Stage } from 'src/app/models/stage';
import * as moment from 'moment';
import { Employee } from 'src/app/models/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { Assignment } from 'src/app/models/assignment';
import { Subpart } from 'src/app/models/subpart';
import { SubpartsComponent } from 'src/app/subparts/subparts.component';
import { CustomValidators } from 'src/app/common/custom-validators';

@Component({
  selector: 'tt-assignment-edit',
  templateUrl: './assignment-edit.component.html',
  styleUrls: ['./assignment-edit.component.scss']
})
export class AssignmentEditComponent implements OnInit {
  @Input() boardId?: string;
  @Input() assignmentId?: string;
  @Input() sidebarView: boolean = false;
  @ViewChild(SubpartsComponent) subpartsComponent!: SubpartsComponent;

  form!: FormGroup;
  isFormValid: boolean = false;

  stages: Stage[] = [];
  subparts: Subpart[] = [];
  employees: Employee[] = [];
  mode: string = "edit";
  
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private assignmentService: AssignmentService,
    private employeeService: EmployeeService,
    private stageService: StageService) { 
      
    }
  
  ngOnInit(): void {
    this.setFields();
    this.initiateForm();
    this.prepareData();
  }

  private setFields() {
    this.boardId ??= this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId ??= this.activatedRoute.snapshot.paramMap.get('taskId') ?? "0";
    this.mode = this.assignmentId == "0" ? "create" : "edit";
  }

  private initiateForm() {
    this.form = new FormGroup({
      id: new FormControl(0, [
        Validators.required
      ]),
      topic: new FormControl("", [
        Validators.required,
        Validators.maxLength(50)
      ]),
      description: new FormControl(),
      stage: new FormControl(null, [
        Validators.required
      ]),
      deadlineDate: new FormControl(new Date(), [
        Validators.required
      ]),
      deadlineTime: new FormControl("23:00"),
      responsibleEmployeeId: new FormControl("", [
        Validators.required
      ]),
      isCompleted: new FormControl(false)
    }, {
      validators: CustomValidators.dateInTheFutureValidator()
    }
    );
  }

  private prepareData() {
    this.getStages();
    this.getEmployees();
    if (this.mode === "edit") {
      this.loadAssignment();
    }
    else {
      this.subparts = [];
    }
  }

  private loadAssignment() {
    this.assignmentService.getAssignment(this.boardId!, this.assignmentId!)
      .subscribe(result => {
        this.form.patchValue(result);
        this.form.patchValue({ 'deadlineDate': result.deadline });
        this.form.patchValue({ 'deadlineTime': this.getTimeFromDateTime(result.deadline) });
        this.form.patchValue({ 'stage': result.stageId });
        this.form.patchValue({ 'responsibleEmployeeId': result.responsibleEmployeeId });

        this.subparts = result.subparts;
      });
  }

  public updateStage() {
    this.assignmentService.getAssignment(this.boardId!, this.assignmentId!)
      .subscribe(result => {
        this.form.patchValue({ 'stage': result.stageId });
      });
  }

  private getStages() {
    this.stageService.getStages(this.boardId!)
      .subscribe(stages => {
        this.stages = stages;
      });
  }

  private getEmployees() {
    this.employeeService.getEmployees(this.boardId!)
      .subscribe(result => {
        this.employees = result;
        this.employees.sort((a, b) => {
          let compareResult = a.firstName?.localeCompare(b.firstName!);
          if(compareResult) return compareResult;
          else return 0;
        });
      })
  }

  onSubmit() {
    this.isFormValid = this.subpartsComponent.areAllSubpartsValid() && this.form.valid;
    if(this.isFormValid)
    {
      let assignment: Assignment = {
        id: this.form.controls['id'].value,
        topic: this.form.controls['topic'].value,
        description: this.form.controls['description'].value,
        boardId: Number(this.boardId),
        stageId: this.form.controls['stage'].value,
        deadline: this.getDeadline(),
        responsibleEmployeeId: this.form.controls['responsibleEmployeeId'].value,
        isCompleted: this.form.controls['isCompleted'].value,
        subparts: this.subpartsComponent.getSubparts()
      };
      
      if (this.mode === "create")
        this.createAssignment(assignment);
      else if (this.mode === "edit")
        this.updateAssignment(assignment);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private getDeadline() : Date {
      let deadlineDate = this.form.controls['deadlineDate'].value;
      
      let deadline : moment.Moment = moment.isMoment(deadlineDate)
        ? moment(deadlineDate)
        : moment(deadlineDate, 'YYYY-MM-DDTHH:mm:ss'); 

      let deadlineTime = this.form.controls['deadlineTime'].value;
      let timeMoment = moment(deadlineTime, 'HH:mm');

      deadline = moment(deadline).hour(0).minute(0)
        .add(timeMoment.hours() + deadline.utcOffset() / 60, 'hours')
        .add(timeMoment.minutes(), 'minutes');

      return deadline.toDate();
  }

  private getTimeFromDateTime(dateTime: Date | undefined) : string {
    if (!dateTime) return "00:00";
    let momentDateTime = moment(dateTime, 'YYYY-MM-DDTHH:mm:ss');
    return momentDateTime.hours() + ":" + momentDateTime.minutes();
  }

  private createAssignment(assignment: any) {
    this.assignmentService.createAssignment(this.boardId!, assignment)
      .subscribe( () => { 
        if(!this.sidebarView) {
          this.router.navigate(['/boards', this.boardId])
            .catch(error => console.log(error))
        }
    });
  }

  private updateAssignment(assignment: any) {
    
    this.assignmentService.updateAssignment(this.boardId!, assignment)
      .subscribe( () => { 
        if(!this.sidebarView) {
          this.router.navigate(['/boards', this.boardId, 'tasks', this.assignmentId])
            .catch(error => console.log(error))
        }
      });
  }
}
