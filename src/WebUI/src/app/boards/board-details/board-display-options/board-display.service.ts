import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { Assignment } from "src/app/models/assignment";

@Injectable({
    providedIn: 'root',
  })
export class BoardDisplayService {
    
    private _sortFieldKey: string = "sortField";
    private _sortOrderKey: string = "sortOrder";

    private _sortingFunction = new Subject<AssignmentComparator>();
    public sortingFunction = this._sortingFunction.asObservable();

    private _showOnlyMyTasksKey: string = "onlyMyTasks";
    private _showCompletedTasksKey: string = "completedTasks";

    private _filterFunction = new Subject<AssignmentFilter>();
    public filterFunction = this._filterFunction.asObservable();

    getShowOnlyMyTasks() {
        return localStorage.getItem(this._showOnlyMyTasksKey) ?? "false";
    }

    getShowCompletedTasks() {
        return localStorage.getItem(this._showCompletedTasksKey) ?? "true";
    }

    setShowOnlyMyTasks(showOnlyMyTasks: boolean) {
        localStorage.setItem(this._showOnlyMyTasksKey, showOnlyMyTasks.toString());
    }

    setShowCompletedTasks(showCompletedTasks: boolean) {
        localStorage.setItem(this._showCompletedTasksKey, showCompletedTasks.toString());
    }

    doFiltration() {
        let showOnlyMyTasks = this.getShowOnlyMyTasks();
        let showCompletedTasks = this.getShowCompletedTasks();
        switch(true) {
            case showOnlyMyTasks === 'true' && showCompletedTasks === 'true':
                this._filterFunction.next(this.filterAllMyTasks);
                break;
            case showOnlyMyTasks === 'true' && showCompletedTasks === 'false':
                this._filterFunction.next(this.filterMyIncompletedTasks);
                break;
            case showOnlyMyTasks === 'false' && showCompletedTasks === 'true':
                this._filterFunction.next(this.filterAllTasks);
                break;
            case showOnlyMyTasks === 'false' && showCompletedTasks === 'false':
                this._filterFunction.next(this.filterAllIncompletedTasks);
                break;
            default:
                this._filterFunction.next(this.filterAllTasks);
                break;
        }
    }

    getSortField() {
        return localStorage.getItem(this._sortFieldKey) ?? "name";
    }

    setSortField(field: string) {
        localStorage.setItem(this._sortFieldKey, field);
    }

    getSortOrder() {
        return localStorage.getItem(this._sortOrderKey) ?? "asc";
    }

    setSortOrder(order: string) {
        localStorage.setItem(this._sortOrderKey, order);
    }

    doSorting() {
        let field = this.getSortField();
        let order = this.getSortOrder();
        switch (true) {
            case field === 'name' && order === 'asc': 
                this._sortingFunction.next(this.sortTasksByNameAsc);
                break;
            case field === 'name' && order === 'desc': 
                this._sortingFunction.next(this.sortTasksByNameDesc);
                break;
            case field === 'deadline' && order === 'asc':
                this._sortingFunction.next(this.sortTasksByDeadlineAsc);
                break;
            case field === 'deadline' && order === 'desc':
                this._sortingFunction.next(this.sortTasksByDeadlineDesc);
                break;
            default:
                this._sortingFunction.next(this.sortTasksByNameAsc);
                break;
        }
    }

    private sortTasksByNameAsc(a: Assignment, b: Assignment) {
        return a.topic.toLowerCase() > b.topic.toLowerCase() ? 1 : -1;
    }

    private sortTasksByNameDesc(a: Assignment, b: Assignment) {
        return a.topic.toLowerCase() > b.topic.toLowerCase() ? -1 : 1;
    }


    private sortTasksByDeadlineAsc(a: Assignment, b: Assignment) {
        if (a.deadline) {
            if (b.deadline)
                return a.deadline > b.deadline ? 1 : -1;
            else
                return 1; 
        }
        return b.deadline ? -1 : 0;
    }

    private sortTasksByDeadlineDesc(a: Assignment, b: Assignment) {
        if (a.deadline) {
            if (b.deadline)
                return a.deadline > b.deadline ? -1 : 1;
            else
                return -1; 
        }
        return b.deadline ? 1 : 0;
    }

    private filterMyIncompletedTasks(a: Assignment, employeeId?: number) {
        if (!a || !employeeId) 
            return false;
        
        return a.responsibleEmployeeId === employeeId && !a.isCompleted;
    }
    private filterAllMyTasks(a: Assignment, employeeId?: number) {
        if (!a || !employeeId) 
            return false;
        
        return a.responsibleEmployeeId === employeeId;
    }
    private filterAllIncompletedTasks(a: Assignment, employeeId?: number) {
        return !a.isCompleted;
    }
    private filterAllTasks(a: Assignment, employeeId?: number) {
        return true;
    }
}

export type AssignmentComparator = (a: Assignment, b: Assignment) => number;
export type AssignmentFilter = (a: Assignment, employeeId?: number) => boolean;