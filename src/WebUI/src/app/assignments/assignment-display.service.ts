import { Injectable } from "@angular/core";
import { AssignmentDisplayModel } from "../models/display-models/assignment-display-model";
import { AuthService } from "../auth/auth.service";
import { Board } from "../models/board";
import { Stage } from "../models/stage";
import { Employee } from "../models/employee";
import * as moment from 'moment';
import { Assignment } from "../models/assignment";

@Injectable({
    providedIn: 'root',
  })

export class AssignmentDisplayService {
  constructor(private authService: AuthService) {
  }
  
  getAssignmentDisplayModels(boards: Board[]): AssignmentDisplayModel[] {
    const assignmentDetails: AssignmentDisplayModel[] = [];
    for (const board of boards) {
      assignmentDetails.push(...this.getAssignmentDetails(board));
    }
    return (this.authService.isAdmin() || this.authService.isManager())
      ? assignmentDetails
      : this.getAssignmentModelsOfTheEmployee(assignmentDetails); 
  }
  
  private getAssignmentDetails(board: Board): AssignmentDisplayModel[] {
    if (!board.assignments) {
        return [];
    }
    const assignmentDetails: AssignmentDisplayModel[] = [];
    for (const assignment of board.assignments) {
      const detail: AssignmentDisplayModel = {
        id: assignment.id,
        boardId: assignment.boardId,
        boardName: board.name,
        topic: assignment.topic,
        description: assignment.description,
        deadline: assignment.deadline,
        isCompleted: assignment.isCompleted,
        stage: this.getStage(board, assignment.stageId),
        responsibleEmployee: this.getEmployee(board, assignment.responsibleEmployeeId)
      }
      assignmentDetails.push(detail);
    }
    return assignmentDetails;
  }
  
  private getStage(board: Board, stageId: number): Stage | undefined {
    return board.stages?.find(s => s.id == stageId);
  }

  private getEmployee(board: Board, employeeId: number): Employee | undefined {
    return board.employees?.find(e => e.id == employeeId);
  }

  private getAssignmentModelsOfTheEmployee(assignmentDetails: AssignmentDisplayModel[])
    : AssignmentDisplayModel[] {
      const employeeId = this.authService.getEmployeeId()
      if (!employeeId) {
        return [];
      }
      return assignmentDetails.filter(ad => ad.responsibleEmployee 
        && ad.responsibleEmployee.id.toString() == employeeId)
  }

  getDeadline(deadlineDate: any, deadlineTime: any) : Date {
    
    let deadline : moment.Moment = moment.isMoment(deadlineDate)
      ? moment(deadlineDate)
      : moment(deadlineDate, 'YYYY-MM-DDTHH:mm:ss'); 

    const timeMoment = moment(deadlineTime, 'HH:mm');

    deadline = moment(deadline).hour(0).minute(0)
      .add(timeMoment.hours() + deadline.utcOffset() / 60, 'hours')
      .add(timeMoment.minutes(), 'minutes');

    return deadline.toDate();
  }

  getTimeFromDateTime(dateTime: Date | undefined) : string {
    if (!dateTime) {
      return "00:00";
    }
    const momentDateTime = moment(dateTime, 'YYYY-MM-DDTHH:mm:ss');
    return momentDateTime.hours() + ":" + momentDateTime.minutes();
  }

  isUserAuthorizeToModifyTask(assignment: Assignment): boolean {
    return this.authService.isAdmin() || this.authService.isManager() ||
      (this.authService.isEmployee()
        && this.authService.getEmployeeId() !== null
        && assignment != undefined
        && this.authService.getEmployeeId() === assignment.responsibleEmployeeId.toString());
  }
}