import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Assignment } from "../models/assignment";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root',
  })
export class AssignmentService {
    
  constructor(private http: HttpClient) {

  }

  getAssignments(boardId: number | string) : Observable<Assignment[]> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/";
    return this.http.get<Assignment[]>(url);
  }

  getAssignment(boardId: number | string, assignmentId: number | string) : Observable<Assignment> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/" + assignmentId;
    return this.http.get<Assignment>(url);
  }

  createAssignment(boardId: number | string, assignment: Assignment): Observable<Object> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/";
    return this.http.post(url, assignment);
  }

  updateAssignment(boardId: number | string, assignment: Assignment): Observable<Object> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/" + assignment.id;
    return this.http.put(url, assignment);
  }

  moveAssignmentToTheStage(boardId: number | string, assignmentId: number | string,
    stageId: number | string): Observable<Object> {
      const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/" + assignmentId + "/move/" + stageId;
      return this.http.put(url, null);
  }

  changeAssignmentStatus(boardId: number | string, assignmentId: number | string,
    isCompleted: boolean): Observable<Object> {
      let url = environment.baseUrl + "api/boards/" + boardId + "/tasks/" + assignmentId + "/";
      if (isCompleted) {
        url += "complete";
      } else {
        url += "uncomplete";
      }
      return this.http.put(url, null);
  }

  deleteAssignment(boardId: number | string, assignmentId: number | string): Observable<Object> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/tasks/" + assignmentId;
    return this.http.delete(url);
  }
}