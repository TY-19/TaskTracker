import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";

export class MatTableHelper<T extends Record<string, any>> {
    
    private isSortInitiated: boolean = false;
    private isPaginationInitiated: boolean = false;

    initiateTable(dataSource?: MatTableDataSource<T>, sort?: MatSort, paginator?: MatPaginator) {
        if (dataSource) {
            this.initiateSort(dataSource, sort);
            this.initiatePagination(dataSource, paginator);
        }
    }

    private initiatePagination(dataSource?: MatTableDataSource<T>, paginator?: MatPaginator) {
        if (!this.isPaginationInitiated && paginator) {
          dataSource!.paginator = paginator;
          this.isPaginationInitiated = true;
        }
      }
    
    private initiateSort(dataSource?: MatTableDataSource<T>, sort?: MatSort) {
        if (!this.isSortInitiated && sort) {
            sort.disableClear = true;
            dataSource!.sort = sort;
            dataSource!.sortingDataAccessor = 
               (data, sortHeaderId) => data[sortHeaderId].toLowerCase();
            this.isSortInitiated = true;
        }
      }
}