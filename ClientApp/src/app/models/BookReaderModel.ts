import { BookModel } from "./BookModel";
import { ReaderModel } from "./ReaderModel";

export class BookReaderModel{
    public id: number = 0;
    public bookId: number = 0;
    public readerId: number = 0;
    public dateOfIssue: string = '';
    public book: BookModel = new BookModel();
    public reader: ReaderModel = new ReaderModel();
}