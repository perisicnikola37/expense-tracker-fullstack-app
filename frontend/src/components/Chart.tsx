import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
} from "recharts";

const mockApiResponse = {
    Expenses: [1, 2, 3, 0, 2, 6, 3],
    Incomes: [2, 3, 4, 5, 6, 7, 3],
};

const generateData = () => {
    const currentDate = new Date();
    const data = [];

    for (let i = 6; i >= 0; i--) {
        const date = new Date(currentDate);
        date.setDate(currentDate.getDate() - i);

        const dateString = `${date.getDate()}.${date.getMonth() + 1}.`;

        const newDataPoint = {
            name: dateString,
            Incomes: mockApiResponse.Incomes[i],
            Expenses: mockApiResponse.Expenses[i],
        };

        data.push(newDataPoint);
    }

    return data;
};

export default function ChartReacharts() {
    const data = generateData();

    return (
        <>
            <LineChart
                className="_chart m-auto mt-10"
                width={600}
                height={300}
                data={data}
                margin={{
                    top: 5,
                    right: 30,
                    left: 20,
                    bottom: 5,
                }}
            >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Legend />

                <Line
                    type="monotone"
                    dataKey="Incomes"
                    stroke="#8884d8"
                    activeDot={{ r: 8 }}
                />

                <Line
                    type="monotone"
                    dataKey="Expenses"
                    stroke="#82ca9d"
                    activeDot={{ r: 8 }}
                />
            </LineChart>
        </>
    );
}