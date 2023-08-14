import { Component, OnInit } from '@angular/core';
import { BoardDisplayService } from './board-display.service';

@Component({
  selector: 'tt-board-display-options',
  templateUrl: './board-display-options.component.html',
  styleUrls: ['./board-display-options.component.scss']
})
export class BoardDisplayOptionsComponent implements OnInit {
  sortingFieldSelected: string = "name";
  sortingOrderSelected: string = "asc";
  showOnlyMyTasksSelected: boolean = false;
  showCompletedTasksSelected: boolean = true;
  
  constructor(private boardDisplayService: BoardDisplayService) { 

  }

  ngOnInit(): void {
    this.setFields();
  }

  private setFields(): void {
    this.sortingFieldSelected = this.boardDisplayService.getSortField();
    this.sortingOrderSelected = this.boardDisplayService.getSortOrder();
    this.showOnlyMyTasksSelected = this.boardDisplayService.getShowOnlyMyTasks();
    this.showCompletedTasksSelected = this.boardDisplayService.getShowCompletedTasks();
  }

  sortBy(sortingField: string): void {
    this.boardDisplayService.setSortField(sortingField);
    this.boardDisplayService.doSorting();
  }

  sortInOrder(order: string): void {
    this.boardDisplayService.setSortOrder(order);
    this.boardDisplayService.doSorting();
  }

  changeShowOnlyMyTasks(checked: boolean): void {
    this.boardDisplayService.setShowOnlyMyTasks(checked);
    this.boardDisplayService.doFiltration();
  }

  changeShowCompletedTasks(checked: boolean): void {
    this.boardDisplayService.setShowCompletedTasks(checked);
    this.boardDisplayService.doFiltration();
  }
}
