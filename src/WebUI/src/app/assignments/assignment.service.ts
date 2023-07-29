import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Assignment } from "../models/assignment";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root',
  })
export class AssignmentService {
    
  constructor(private http: HttpClient) {

  }

  getAssignments(boardId: string) : Observable<Assignment[]> {
    const url = "/api/boards/" + boardId + "/tasks/";
    return this.http.get<Assignment[]>(url);
  }

  getAssignment(boardId: string, assignmentId: string) : Observable<Assignment> {
    const url = "/api/boards/" + boardId + "/tasks/" + assignmentId;
    return this.http.get<Assignment>(url);
  }

  createAssignment(boardId: string, assignment: Assignment) {
    const url = "/api/boards/" + boardId + "/tasks/";
    return this.http.post(url, assignment);
  }

  updateAssignment(boardId: string, assignment: Assignment) {
    const url = "/api/boards/" + boardId + "/tasks/" + assignment.id;
    return this.http.put(url, assignment);
  }

  moveAssignmentToTheStage(boardId: number | string, 
    assignmentId: number | string, stageId: number | string) {
      const url = "/api/boards/" + boardId + "/tasks/" + assignmentId + "/move/" + stageId;
      return this.http.put(url, null);
  }

  changeAssignmentStatus(boardId: number | string, 
    assignmentId: number | string, isCompleted: boolean) {
      let url = "/api/boards/" + boardId + "/tasks/" + assignmentId + "/";
      if (isCompleted) {
        url += "complete";
      } else {
        url += "uncomplete";
      }
      return this.http.put(url, null);
  }

  deleteAssignment(boardId: string, assignmentId: string) {
    const url = "/api/boards/" + boardId + "/tasks/" + assignmentId;
    return this.http.delete(url);
  }
}