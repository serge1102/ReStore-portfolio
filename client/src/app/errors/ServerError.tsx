import { Button, Container, Divider, Paper, Typography } from '@mui/material'
import { useLocation, useNavigate } from 'react-router-dom'

export default function ServerError() {
    const navigate = useNavigate(); // v6 から useHistoryではなく useNavigate
    const { state } = useLocation(); // agent.ts から渡したオブジェクトは state としてアクセスできる
    return (
        <Container component={Paper}>
            {(state as any)?.error ? (
                <>
                    <Typography variant='h3' color='error' gutterBottom>{(state as any).error.title}</Typography>
                    <Divider />
                    <Typography>{(state as any).error.detail || 'Internal server error'}</Typography>
                </>
            ) : (
                <Typography variant='h5' gutterBottom>Server error</Typography>
            )}
            <Button onClick={() => navigate('/catalog')}>Go back to the store</Button>
        </Container>
    )
}
