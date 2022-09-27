export const INCREMENT_COUNTER = "INCREMENT_COUNTER";
export const DECREMENT_COUNTER = "DECREMENT_COUNTER";

export interface CounterState {
  title: string;
  data: number;
}

const initialState: CounterState = {
  title: "これはタイトル",
  data: 42,
};

// Action Creator
export function increment(amount = 1) {
  return {
    type: INCREMENT_COUNTER,
    payload: amount,
  };
}

// Action Creator
export function decrement(amount = 1) {
  return {
    type: DECREMENT_COUNTER,
    payload: amount,
  };
}

export default function counterReducer(state = initialState, action: any) {
  switch (action.type) {
    case INCREMENT_COUNTER:
      // return state.data + 1; みたいなことは state の mutate なのでやってはいけない
      // スプレッド演算子で state をコピーし、新しい state を作る
      return {
        ...state,
        data: state.data + action.payload,
      };
    case DECREMENT_COUNTER:
      return {
        ...state,
        data: state.data - action.payload,
      };
    // state を更新しない場合でも state を返すようにする
    // 特に初回は action がないので initialState のセットのため
    default:
      return state;
  }
}
