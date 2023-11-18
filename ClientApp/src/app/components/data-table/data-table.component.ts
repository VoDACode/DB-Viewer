import { Component, Input } from '@angular/core';
import { ColumnType, TablePattern } from 'src/app/models/TablePattern';

type RowType = {
  data: any,
  editMode: boolean,
  editData: any
}

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.css']
})
export class DataTableComponent {
  @Input() 
  public tablePattern: TablePattern | undefined = undefined;

  private _prevTablePattern: TablePattern | undefined = undefined;

  private _columns: ColumnType[] = [];

  private _rows: RowType[] = [];

  public newEntity: any = {};

  constructor() { }

  public get columns(): ColumnType[] {
    if(this.tablePattern === undefined)
      return [];

    if (this._columns.length === 0 || this._prevTablePattern !== this.tablePattern) {
      console.log('Load data for table pattern: ', this.tablePattern.api.url);
      this._prevTablePattern = this.tablePattern;
      this._columns = this.tablePattern.columns;
      this.tablePattern.api.getAll().subscribe((data: any[]) => {
        this._rows = data.map((row: any): RowType => {
          return {
            data: row,
            editMode: false,
            editData: {
              ...row
            }
          }
        });
      });
    }
    return this._columns;
  }
  public get rows(): RowType[] {
    return this._rows;
  }

  public columnIsObject(column: ColumnType): boolean {
    return typeof column.type !== "string";
  }

  public showObject(column: ColumnType, row: RowType): void {
    alert(JSON.stringify(row.data[column.name], null, 2));
  }

  public add(): void {
    this.tablePattern?.api.create(this.newEntity).subscribe((data: any) => {
      this._rows.push({
        data,
        editMode: false,
        editData: {
          ...data
        }
      });
      this.newEntity = {};
    });
  }

  public edit(row: RowType): void {
    row.editMode = true;
  }

  public save(row: RowType): void {
    row.editMode = false;
    this.tablePattern?.api.update(row.data.id, row.editData).subscribe((data: any) => {
      row.data = data;
    });
  }

  public cancel(row: RowType): void {
    row.editMode = false;
    row.editData = {
      ...row.data
    };
  }

  public delete(row: RowType): void {
    this.tablePattern?.api.delete(row.data.id).subscribe(() => {
      this._rows = this._rows.filter(_row => _row !== row);
    });
  }
}
