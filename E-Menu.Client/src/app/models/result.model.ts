export class ResultModel<T> {
  isSuccess: boolean = false;
  Error: string = "";
  value?: T | null;
  statusCode: number = 0;
}
