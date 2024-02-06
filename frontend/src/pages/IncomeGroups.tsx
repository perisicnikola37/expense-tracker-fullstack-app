import { useEffect } from "react";
import { Helmet } from "react-helmet";
import { useModal } from "../contexts/GlobalContext";
import useObjectGroups from "../hooks/GlobalHooks/GetObjectsHook";
import EnhancedTable from "../components/Tables/IncomeGroupsTable";

const IncomeGroups = () => {
    const { objectGroups, fetchObjectGroups } = useObjectGroups("income");
    const { actionChange } = useModal();

    useEffect(() => {
        fetchObjectGroups();
    }, [actionChange]);

    return (
        <div className="w-full max-w-screen-xl min-h-[48rem] mx-auto p-4 md:py-8">
            <Helmet>
                <title>Income groups | Expense Tracker</title>
            </Helmet>
            <EnhancedTable incomeGroups={objectGroups} />
        </div>
    );
}

export default IncomeGroups;