import { Button, ButtonGroup, Typography } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { increment, decrement } from "./counterSlice";


export default function ContactPage() {
  const dispatch = useAppDispatch();
  const { title, data } = useAppSelector(state => state.counter);
  return (
    <>
      <Typography variant="h2">{title}</Typography>
      <Typography variant="h5">The data is : {data}</Typography>
      <ButtonGroup>
        <Button onClick={() => dispatch(increment(1))}>+</Button>
        <Button onClick={() => dispatch(increment(5))}>+5</Button>
        <Button onClick={() => dispatch(decrement(1))}>-</Button>
        <Button onClick={() => dispatch(decrement(5))}>-5</Button>
      </ButtonGroup>
    </>
  );
}

