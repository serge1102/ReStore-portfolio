import { createSlice } from "@reduxjs/toolkit";

export interface CounterState {
  title: string;
  data: number;
}   

const initialState: CounterState = {
  title: "これはタイトル。Redux Toolkit とともに。",
  data: 42,
};

export const counterSlice = createSlice({
  name: "counter",
  initialState,
  reducers: {
    increment: (state, action) => {
      // redux toolkit では mutable なロジックにしても immutable に変更を加えてくれるため
      // 意識して immutable なロジックを実装しなくてよい
      state.data += action.payload;
    },
    decrement: (state, action) => {
      state.data -= action.payload;
    },
  },
});

// redux toolkit が action を作成してくれる
export const {increment, decrement} = counterSlice.actions;